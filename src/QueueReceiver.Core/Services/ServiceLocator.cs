using Microsoft.Extensions.DependencyInjection;
using QueueReceiver.Core.Interfaces;

namespace QueueReceiver.Core.Services
{
    /// <summary>
    /// A wrapper around ServiceScopeFactory to make it easier to fake out with MOQ.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/53509491/54288"/>
    public sealed class ServiceLocator : IServiceLocator //TODO: No need for this class - remove.
    {
        private readonly IServiceScopeFactory _factory;
        private IServiceScope? _scope;

        public ServiceLocator(IServiceScopeFactory factory)
            => _factory = factory;

        public T GetScope<T>()
        {
            CreateScope();

            return _scope!.ServiceProvider.GetService<T>();
        }

        public IServiceScope CreateScope()
            => _factory.CreateScope();

        public void Dispose()
        {
            _scope?.Dispose();
            _scope = null;
        }
    }
}