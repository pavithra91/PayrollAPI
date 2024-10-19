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
        public BackgroudService(IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue)
        {
            _serviceProvider = serviceProvider;
            _taskQueue = taskQueue;
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

                }
            }
        }

        private Task BackgroundProcess(PaysheetBGParams workItem)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbConnect = scope.ServiceProvider.GetRequiredService<DBConnect>();
                PaysheetPrint paysheet = new PaysheetPrint();
                paysheet.PrintPaySheets(workItem.companyCode, workItem.period, workItem.approvedBy, dbConnect);
            }

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