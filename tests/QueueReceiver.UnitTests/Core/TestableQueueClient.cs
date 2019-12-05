using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace QueueReceiver.UnitTests.Core
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public class TestableQueueClient : IQueueClient
    {
        private Func<Message, CancellationToken, Task> processMessagesAsync;

        public async Task SendMessage(Message message, CancellationToken cancellationToken)
        {
            await processMessagesAsync(message, cancellationToken);
        }

        public void RegisterMessageHandler(
            Func<Message,
            CancellationToken,
            Task> processMessagesAsync,
            MessageHandlerOptions messageHandlerOptions)
        {
            this.processMessagesAsync = processMessagesAsync;
        }

        #region NotImplemented
        string IQueueClient.QueueName => throw new NotImplementedException();

        int IReceiverClient.PrefetchCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ReceiveMode IReceiverClient.ReceiveMode => throw new NotImplementedException();

        string IClientEntity.ClientId => throw new NotImplementedException();

        bool IClientEntity.IsClosedOrClosing => throw new NotImplementedException();

        string IClientEntity.Path => throw new NotImplementedException();

        TimeSpan IClientEntity.OperationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ServiceBusConnection IClientEntity.ServiceBusConnection => throw new NotImplementedException();

        bool IClientEntity.OwnsConnection => throw new NotImplementedException();

        IList<ServiceBusPlugin> IClientEntity.RegisteredPlugins => throw new NotImplementedException();

        Task IReceiverClient.AbandonAsync(string lockToken, IDictionary<string, object> propertiesToModify)
        {
            throw new NotImplementedException();
        }

        Task ISenderClient.CancelScheduledMessageAsync(long sequenceNumber)
        {
            throw new NotImplementedException();
        }

        Task IClientEntity.CloseAsync()
        {
            throw new NotImplementedException();
        }

        Task IReceiverClient.CompleteAsync(string lockToken)
        {
            throw new NotImplementedException();
        }

        Task IReceiverClient.DeadLetterAsync(string lockToken, IDictionary<string, object> propertiesToModify)
        {
            throw new NotImplementedException();
        }

        Task IReceiverClient.DeadLetterAsync(string lockToken, string deadLetterReason, string deadLetterErrorDescription)
        {
            throw new NotImplementedException();
        }

        void IReceiverClient.RegisterMessageHandler(Func<Message, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            throw new NotImplementedException();
        }

        void IClientEntity.RegisterPlugin(ServiceBusPlugin serviceBusPlugin)
        {
            throw new NotImplementedException();
        }

        void IQueueClient.RegisterSessionHandler(Func<IMessageSession, Message, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            throw new NotImplementedException();
        }

        void IQueueClient.RegisterSessionHandler(Func<IMessageSession, Message, CancellationToken, Task> handler, SessionHandlerOptions sessionHandlerOptions)
        {
            throw new NotImplementedException();
        }

        Task<long> ISenderClient.ScheduleMessageAsync(Message message, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            throw new NotImplementedException();
        }

        Task ISenderClient.SendAsync(Message message)
        {
            throw new NotImplementedException();
        }

        Task ISenderClient.SendAsync(IList<Message> messageList)
        {
            throw new NotImplementedException();
        }

        void IClientEntity.UnregisterPlugin(string serviceBusPluginName)
        {
            throw new NotImplementedException();
        }
        #endregion 
    }
}
