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
        private readonly IServiceLocator _serviceLocator;
        private readonly ILogger<EntryPointService> _logger;


        public EntryPointService(IQueueClient queueClient,
            IServiceLocator serviceLocator,
            ILogger<EntryPointService> logger)
        {
            _queueClient = queueClient;
            _serviceLocator = serviceLocator;
            _logger = logger;
        }

        public Task InitializeQueueAsync()
        {
            RegisterOnMessageHandlerAndReceiveMessages();
            return Task.CompletedTask;
        }

        public async Task DisposeQueueAsync()
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
            using (_serviceLocator)
            {
                var accessService = _serviceLocator.GetService<IAccessService>();
                var accessInfo = JsonConvert.DeserializeObject<AccessInfo>(Encoding.UTF8.GetString(message.Body));

                await accessService.HandleRequestAsync(accessInfo);

                //Locktoken now throws exception in tests as it's internal set (and sealed), and not possible to mock
                string lockToken = message.SystemProperties.LockToken;
                await _queueClient.CompleteAsync(lockToken);
                _logger.LogInformation(Resources.MessageSuccess);
            }
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
