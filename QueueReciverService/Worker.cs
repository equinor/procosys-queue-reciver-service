using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueReceiverService.Models;
using QueueReceiverService.Services;

namespace QueueReceiverService
{
    public class Worker : BackgroundService
    {
        private readonly QueueClient _queueClient;
        private const string QueueName = "updateuseraccessdev";
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<Worker> _logger;
        private bool _isRegistered;
        public IConfiguration Configuration { get; }

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            _scopeFactory = scopeFactory;
            var connectionString = Configuration["ServiceBusConnectionString"];
            _queueClient = new QueueClient(connectionString, QueueName);
            _queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            _isRegistered = true;
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            bool isSuccess;
            var accessInfo = JsonConvert.DeserializeObject<AccessInfo>(Encoding.UTF8.GetString(message.Body));
            _logger.LogInformation($"Processing message : { accessInfo }");

            using (var scope = _scopeFactory.CreateScope())
            {
                var accessService = scope.ServiceProvider.GetRequiredService<IAccessService>();
                isSuccess = await accessService.HandleRequest(accessInfo);
            }

            if (isSuccess)
            {
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogDebug($"- Endpoint: {context.Endpoint}");
            _logger.LogDebug($"- Entity Path: {context.EntityPath}");
            _logger.LogDebug($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }

        public async Task CloseQueueAsync()
        {
            await _queueClient.CloseAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_isRegistered)
                {
                    RegisterOnMessageHandlerAndReceiveMessages();
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
