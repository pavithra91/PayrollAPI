using Microsoft.Extensions.DependencyInjection;
using PayrollAPI.Interfaces.Reservation;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class RaffleConfirmationJob : IJob
    {
        private readonly IReservation _reservation;
        private readonly ILogger<RaffleConfirmationJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RaffleConfirmationJob(ILogger<RaffleConfirmationJob> logger, IReservation reservation, IServiceProvider serviceProvider)
        {
            _reservation = reservation;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _reservation.WinnerConfirmation();
        }
    }
}
