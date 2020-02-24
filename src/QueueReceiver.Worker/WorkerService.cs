using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.Worker
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IEntryPointService _entryPointService;

        public WorkerService(ILogger<WorkerService> logger, IEntryPointService entryPointService)
        {
            _logger = logger;
            _entryPointService = entryPointService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker service at: {DateTimeOffset.Now}");
            await _entryPointService.InitializeQueueAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: { DateTimeOffset.Now}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            await _entryPointService.DisposeQueueAsync();
            _logger.LogInformation($"Worker service stopping at: at: { DateTimeOffset.Now}");
        }
    }
}
