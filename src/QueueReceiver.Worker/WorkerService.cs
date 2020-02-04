using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.Worker
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IServiceLocator _serviceLocator;

        public WorkerService(ILogger<WorkerService> logger, IServiceLocator serviceLocator)
        {
            _logger = logger;
            _serviceLocator = serviceLocator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceLocator.CreateScope();
            var entryPointService =
                scope.ServiceProvider
                    .GetRequiredService<IEntryPointService>();

            _logger.LogInformation($"Worker service at: {DateTimeOffset.Now}");
            await entryPointService.InitializeQueueAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: { DateTimeOffset.Now}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            await entryPointService.DisposeQueueAsync();
            _logger.LogInformation($"Worker service stopping at: at: { DateTimeOffset.Now}");
        }
    }
}
