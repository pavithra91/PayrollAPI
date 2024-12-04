using PayrollAPI.Models.Services;

namespace PayrollAPI.Interfaces
{
    public interface IJobSchedule
    {
        public Task<JobSchedule> GetJobScheduleAsync(string jobName);
        Task UpdateCronExpressionAsync(int jobId, string newCronExpression);
        Task AddJobScheduleAsync(JobSchedule jobSchedule);

        Task RunJobScheduleAsync(string jobName);
        Task PauseJobScheduleAsync(string jobName);
    }
}
