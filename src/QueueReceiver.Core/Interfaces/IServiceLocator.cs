using Microsoft.Extensions.DependencyInjection;
using System;

namespace QueueReceiver.Core.Interfaces
{
    public interface IServiceLocator : IDisposable
    {
        IServiceScope CreateScope();
        T GetService<T>();
    }
}
