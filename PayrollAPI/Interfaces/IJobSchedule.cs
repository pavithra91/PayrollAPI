using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Services;

namespace PayrollAPI.Interfaces
{
    public interface IJobSchedule
    {
        Task<IEnumerable<JobSchedule>> GetAllScheduledJobs();



        public Task<JobSchedule> GetJobScheduleAsync(string jobName);
        Task UpdateCronExpressionAsync(int jobId, string newCronExpression);
        Task AddJobScheduleAsync(JobSchedule jobSchedule);

        Task RunJobScheduleAsync(string jobName);
        Task PauseJobScheduleAsync(string jobName);
    }
}
