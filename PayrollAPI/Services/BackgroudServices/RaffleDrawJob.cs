using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Interfaces.Reservation;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class RaffleDrawJob : IJob
    {
        private readonly IReservation _reservation;
        private readonly ILogger<RaffleDrawJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RaffleDrawJob(ILogger<RaffleDrawJob> logger, IReservation reservation, IServiceProvider serviceProvider)
        {
            _reservation = reservation;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _reservation.RaffelDraw();
        }
    }
}
