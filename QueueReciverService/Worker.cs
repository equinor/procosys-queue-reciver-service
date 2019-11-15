using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using QueueReciverService.Models;
using QueueReciverService.Services;
using Microsoft.Extensions.DependencyInjection;
using QueueReciverService.Data;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace QueueReciverService
{
    public class Worker : BackgroundService
    {
        private readonly IAccessService _accessService;
        private readonly QueueClient _queueClient;
        private const string QUEUE_NAME = "updateuseraccessdev";
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<Worker> _logger;
        private bool isRegistered;
        public IConfiguration Configuration { get; }

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            _scopeFactory = scopeFactory;
            var connectionString = Configuration["ServiceBusConnectionString"];
            _queueClient = new QueueClient(connectionString, QUEUE_NAME);
            _queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProccessMessagesAsync, messageHandlerOptions);
            isRegistered = true;
        }

        private async Task ProccessMessagesAsync(Message message, CancellationToken token)
        {
            //var forTest = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body));
            //_logger.LogInformation($"Proccessing message : {forTest}");
            bool isSuccess= true;

            var accessInfo = JsonConvert.DeserializeObject<AccessInfo>(Encoding.UTF8.GetString(message.Body));
            _logger.LogInformation($"Proccessing message : {accessInfo}");

            using(var scope = _scopeFactory.CreateScope())
            {
                 //var accessService = scope.ServiceProvider.GetRequiredService<IAccessService>();

               // var person = db.Persons.Find(45890);

                isSuccess = await _accessService.HandleRequest(accessInfo);
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
                if (!isRegistered)
                {
                    RegisterOnMessageHandlerAndReceiveMessages();
                }


                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
