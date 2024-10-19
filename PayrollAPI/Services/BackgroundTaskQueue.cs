using PayrollAPI.Interfaces;
using System.Collections.Concurrent;

namespace PayrollAPI.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<PaysheetBGParams> _workItems = new ConcurrentQueue<PaysheetBGParams>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void BackgroundServiceQueue(PaysheetBGParams workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<PaysheetBGParams> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
