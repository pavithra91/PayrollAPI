using Leave.Contracts.Requests;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Services;

namespace PayrollAPI.Interfaces
{
    public interface IJobSchedule
    {
        Task<IEnumerable<JobSchedule>> GetAllScheduledJobs();


        Task<JobSchedule> GetJobScheduleById(int id);
        Task<JobSchedule> GetJobScheduleAsync(string jobName);
        Task<bool> UpdateCronExpressionAsync(int jobId, UpdateScheduleJobRequest request);
        Task AddJobScheduleAsync(JobSchedule jobSchedule);

        Task RunJobScheduleAsync(string jobName);
        Task PauseJobScheduleAsync(string jobName);
    }
}
