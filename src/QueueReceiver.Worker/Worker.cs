using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEntryPointService _entryPointService;

        public Worker(ILogger<Worker> logger, IEntryPointService entryPointService)
        {
            _logger = logger;
            _entryPointService = entryPointService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker service at: {DateTimeOffset.Now}" );
            await _entryPointService.InitializeQueue(); // TODO: Override StartAsync and call this there

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: { DateTimeOffset.Now}");
                await Task.Delay(10000000, stoppingToken); //TODO: Use timespan overload instead to make it easier to read. Timespan.FromSeconds(...) or Timespan.FromMinutes(...) etc.
            }

            await _entryPointService.DisposeQueue(); // TODO: Override StopAsync and call this there
            _logger.LogInformation($"Worker service stopping at: at: { DateTimeOffset.Now}");
        }
    }
}
