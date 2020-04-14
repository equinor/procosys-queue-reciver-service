using Microsoft.Azure.ServiceBus;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests
{
    internal interface ITestableQueueClient
    {
        Task SendMessage(Message message, CancellationToken cancellationToken);
    }
}