using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class BackgroudJob : IJob
    {
        private readonly ILogger<BackgroudJob> _logger;

        public BackgroudJob(ILogger<BackgroudJob> logger)
        {
            _logger = logger;
        }
        Task IJob.Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Job executed at: " + DateTime.Now);
            _logger.LogInformation("Job executed at: " + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
