﻿using Microsoft.Extensions.DependencyInjection;
using QueueReceiver.Core.Interfaces;

namespace QueueReceiver.Core.Services
{
    public sealed class ServiceLocator : IServiceLocator
    {
        private readonly IServiceScopeFactory _factory;
        private IServiceScope? _scope;

        public ServiceLocator(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public T GetService<T>()
        {
            _scope ??= _factory.CreateScope();
            return _scope.ServiceProvider.GetService<T>();
        }

        public IServiceScope CreateScope() => _factory.CreateScope();

        public void Dispose()
        {
            _scope?.Dispose();
            _scope = null;
        }
    }
}
