using Microsoft.Azure.ServiceBus;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.UnitTests.Core
{
    internal interface ITestableQueueClient
    {
        Task SendMessage(Message message, CancellationToken cancellationToken);
    }
}