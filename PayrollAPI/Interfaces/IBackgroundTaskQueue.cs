using PayrollAPI.Services;

namespace PayrollAPI.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void BackgroundServiceQueue(PaysheetBGParams workItem);
        Task<PaysheetBGParams> DequeueAsync(CancellationToken cancellationToken);
    }
}
