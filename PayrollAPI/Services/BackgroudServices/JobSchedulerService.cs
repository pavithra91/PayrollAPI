using PayrollAPI.Models.Services;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class JobSchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public JobSchedulerService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }
        public async void ScheduleJobs(JobSchedule jobSchedule)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            // Create the job and trigger using the cron expression
            var job = JobBuilder.Create<BackgroudJob>()
            .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("MyJobTrigger", jobSchedule.groupName)
                .WithCronSchedule(jobSchedule.cronExpression)  // Set cron expression
                .Build();

            // Schedule the job
            await scheduler.ScheduleJob(job, trigger);
        }

        public async void PauseJobs(JobSchedule jobSchedule)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey(jobSchedule.jobName, jobSchedule.groupName);

            await scheduler.PauseJob(jobKey);
        }
    }
}
