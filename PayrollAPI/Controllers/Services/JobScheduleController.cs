using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Interfaces;
using PayrollAPI.Models.Services;
using PayrollAPI.Repository.Services;
using PayrollAPI.Services.BackgroudServices;
using Quartz;

namespace PayrollAPI.Controllers.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobScheduleController : ControllerBase
    {
        private readonly IJobSchedule _jobSchedule;

        public JobScheduleController(IJobSchedule jobSchedule)
        {
            _jobSchedule = jobSchedule;
        }

        // POST api/job/schedule
        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleJob([FromBody] string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
            {
                return BadRequest("Cron expression cannot be empty.");
            }

            await _jobSchedule.RunJobScheduleAsync("Test Job");
            return Ok("Job scheduled successfully.");
        }

        [HttpPost("pause")]
        public async Task<IActionResult> PauseJob([FromBody] string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                return BadRequest("Job name cannot be empty.");
            }

            await _jobSchedule.PauseJobScheduleAsync("Test Job");
            return Ok("Job scheduled successfully.");
        }
    }
}