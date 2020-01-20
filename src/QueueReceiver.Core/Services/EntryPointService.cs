using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Properties;

namespace QueueReceiver.Core.Services
{
    public class EntryPointService : IEntryPointService
    {
        private readonly IQueueClient _queueClient;
        private readonly IServiceLocator _scopeFactory;
        private readonly ILogger<EntryPointService> _logger;

        public EntryPointService(IQueueClient queueClient,
            IServiceLocator serviceLocator,
            ILogger<EntryPointService> logger)
        {
            _queueClient = queueClient;
            _scopeFactory = serviceLocator;
            _logger = logger;
        }

        public Task InitializeQueue()
        {
            RegisterOnMessageHandlerAndReceiveMessages();
            return Task.CompletedTask;
        }

        public async Task DisposeQueue()
            => await _queueClient.CloseAsync();

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
             * Injecting accessService here because class is singleton.
             * It is not possible inject scoped dependiences from a constructor of a singleton  
            **/
            using var scope = _scopeFactory.CreateScope();
            var accessService =
                scope.ServiceProvider
                    .GetRequiredService<IAccessService>();

            await accessService.HandleRequest(accessInfo);

            //TODO consider moving to its own class, to be able to test, 
            //Locktoken now throws exception in tests as it's internal set (and sealed), and not possible to mock
            string lockToken = message.SystemProperties.LockToken;
            await _queueClient.CompleteAsync(lockToken);
            _logger.LogInformation(Resources.MessageSuccess);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, Resources.MessageHandlerException);
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogDebug($"- Endpoint: {context.Endpoint}");
            _logger.LogDebug($"- Entity Path: {context.EntityPath}");
            _logger.LogDebug($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
