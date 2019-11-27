using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiverService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEntryPointService entryPointService;

        public Worker(ILogger<Worker> logger, IEntryPointService entryPointService)
        {
            _logger = logger;
            this.entryPointService = entryPointService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service at: {time}", DateTimeOffset.Now);
            await entryPointService.InitializeQueue();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000000, stoppingToken);
            }

            await entryPointService.DisposeQueue();
            _logger.LogInformation("Worker service stopping at: {time}", DateTimeOffset.Now);
        }
    }
}
