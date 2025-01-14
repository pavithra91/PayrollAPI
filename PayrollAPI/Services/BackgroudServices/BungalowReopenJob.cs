using PayrollAPI.Interfaces.Reservation;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class BungalowReopenJob : IJob
    {
        private readonly IReservation _reservation;
        private readonly ILogger<BungalowReopenJob> _logger;

        public BungalowReopenJob(ILogger<BungalowReopenJob> logger, IReservation reservation)
        {
            _reservation = reservation;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _reservation.BungalowReopenJob();
        }
    }
}
