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
            IJobDetail? job = null;
            switch (jobSchedule.groupName?.ToLower())
            {
                case "advancepaymentjob":
                    job = JobBuilder.Create<BackgroudJob>()
                        .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                        .Build();
                    break;
                    
                case "tempremovejob":
                    job = JobBuilder.Create<TempApprovalRemoveJob>()
                        .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                        .Build();
                    break;
                case "raffle_draw":
                    job = JobBuilder.Create<RaffleDrawJob>()
                        .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                        .Build();
                    break;

                case "raffle_draw_con":
                    job = JobBuilder.Create<RaffleConfirmationJob>()
                        .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                        .Build();
                    break;

                case "bungalow_reopen":
                    job = JobBuilder.Create<BungalowReopenJob>()
                        .WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
                        .Build();
                    break;

                default:
                    throw new ArgumentException($"Unknown job type: {jobSchedule.jobName}");
            }

            //var job = JobBuilder.Create<BackgroudJob>()
            //.WithIdentity(jobSchedule.jobName, jobSchedule.groupName)
            //    .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"JobTrigger-{jobSchedule.jobName}", jobSchedule.groupName)
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
