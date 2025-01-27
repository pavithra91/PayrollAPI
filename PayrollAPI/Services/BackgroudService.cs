using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayrollAPI.Data;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;

namespace PayrollAPI.Services
{
    public class BackgroudService : IHostedService
    {
        private CancellationTokenSource _cts;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundService> _logger;

        public BackgroudService(IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue, ILogger<BackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task.Run(() => ProcessQueue(_cts.Token));
            return Task.CompletedTask;
        }

        private async Task ProcessQueue(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await BackgroundProcess(workItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(0, $"Error in Background Service : " + ex.Message);
                }
            }
        }

        private Task BackgroundProcess(PaysheetBGParams workItem)
        {
            var scope = _serviceProvider.CreateScope();
            
            var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

            PaysheetPrint paysheet = new PaysheetPrint(dbConnect);

            paysheet.PrintPaySheets(workItem.companyCode, workItem.period, workItem.approvedBy);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }

    public class PaysheetBGParams
    {
        public int companyCode { get; set; }
        public int period { get; set; }
        public string? approvedBy { get; set; }
    }
}