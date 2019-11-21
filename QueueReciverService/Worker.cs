using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
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
        private readonly IQueueClient _queueClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IQueueClient queueClient)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _queueClient = queueClient;
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var accessInfo = JsonConvert.DeserializeObject<AccessInfo>(Encoding.UTF8.GetString(message.Body));
            _logger.LogInformation($"Processing message : { accessInfo }");

            /**
             * Injecting here because Worker is singleton.
             * It not possible initiate scoped dependiences from a constructor of a singleton  
            **/
            using (var scope = _scopeFactory.CreateScope())
            {
                var accessService = scope.ServiceProvider.GetRequiredService<IAccessService>();
                await accessService.HandleRequest(accessInfo);
            }

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            _logger.LogInformation($"Message completed successfully");
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
            await _queueClient.CloseAsync();
        }
    }
}
