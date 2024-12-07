using PayrollAPI.Data;
using PayrollAPI.Interfaces;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Services;
using PayrollAPI.Services.BackgroudServices;
using Quartz;

namespace PayrollAPI.Repository.Services
{
    public class JobScheduleRepository: IJobSchedule
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly HRMDBConnect _context;
        public JobScheduleRepository(HRMDBConnect db, ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            _context = db;
        }

        public async Task<IEnumerable<JobSchedule>> GetAllScheduledJobs()
        {
            return await Task.FromResult(_context.JobSchedule.AsEnumerable());
        }

        public async Task<JobSchedule> GetJobScheduleAsync(string jobName)
        {
            return _context.JobSchedule.FirstOrDefault(js => js.jobName == jobName);
        }

        public async Task UpdateCronExpressionAsync(int jobId, string newCronExpression)
        {
            var jobSchedule = await _context.JobSchedule.FindAsync(jobId);
            if (jobSchedule != null)
            {
                jobSchedule.cronExpression = newCronExpression;
                jobSchedule.lastUpdateDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddJobScheduleAsync(JobSchedule _jobSchedule)
        {
            _context.JobSchedule.Add(_jobSchedule);
            await _context.SaveChangesAsync();

            JobSchedulerService jobScheduler = new JobSchedulerService(_schedulerFactory);
            jobScheduler.ScheduleJobs(_jobSchedule);
        }

        public async Task RunJobScheduleAsync(string jobName)
        {
            JobSchedule jobSchedule = _context.JobSchedule.FirstOrDefault(js => js.jobName == jobName);
            JobSchedulerService jobScheduler = new JobSchedulerService(_schedulerFactory);
            jobScheduler.ScheduleJobs(jobSchedule);
        }

        public async Task PauseJobScheduleAsync(string jobName)
        {
            JobSchedule jobSchedule = _context.JobSchedule.FirstOrDefault(js => js.jobName == jobName);
            JobSchedulerService jobScheduler = new JobSchedulerService(_schedulerFactory);
            jobScheduler.ScheduleJobs(jobSchedule);
        }
    }
}
