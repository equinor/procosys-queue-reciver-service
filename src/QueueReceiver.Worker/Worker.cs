using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var entryPointService =
                scope.ServiceProvider
                    .GetRequiredService<IEntryPointService>();

            _logger.LogInformation($"Worker service at: {DateTimeOffset.Now}" );
            await entryPointService.InitializeQueue();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: { DateTimeOffset.Now}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            await entryPointService.DisposeQueue();
            _logger.LogInformation($"Worker service stopping at: at: { DateTimeOffset.Now}");
        }
    }
}
