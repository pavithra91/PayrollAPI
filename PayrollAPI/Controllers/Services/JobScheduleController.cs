using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Authentication;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Interfaces.Reservation;
using PayrollAPI.Models.Services;
using PayrollAPI.Repository.Services;
using PayrollAPI.Services.BackgroudServices;
using Quartz;

namespace PayrollAPI.Controllers.Services
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class JobScheduleController : ControllerBase
    {
        private readonly IJobSchedule _jobSchedule;

        public JobScheduleController(IJobSchedule jobSchedule)
        {
            _jobSchedule = jobSchedule;
        }


        [HttpGet]
        [Route("get-allScheduledJobs")]
        [ProducesResponseType(typeof(IEnumerable<ScheduleJobResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllScheduledJobs()
        {
            var result = await _jobSchedule.GetAllScheduledJobs();

            var _scheduledJobsResponse = result.MapToResponse();
            return Ok(_scheduledJobsResponse);
        }

        // POST api/job/schedule
        [HttpPost]
        [Route("create-scheduledJob")]
        public async Task<IActionResult> ScheduleJob([FromBody] ScheduleJobRequest request)
        {
            if (request == null)
            {
                return BadRequest("Cron expression cannot be empty.");
            }

            if (!CronExpression.IsValidExpression(request.cronExpression)) 
            {
                return BadRequest("Invalid Cron expression");
            }

            var job = request.MapToJobSchedule();

            await _jobSchedule.AddJobScheduleAsync(job);
            return Ok("Job scheduled successfully.");
        }

        [HttpPut("update-scheduledJob/{id:int}")]
        public async Task<IActionResult> UpdateScheduledJob([FromRoute] int id, [FromBody] UpdateScheduleJobRequest request)
        {
            if (!CronExpression.IsValidExpression(request.cronExpression))
            {
                return BadRequest("Invalid Cron expression");
            }

            var updated = await _jobSchedule.UpdateCronExpressionAsync(id, request);

            if (!updated)
            {
                return NotFound();
            }

            return Ok("success");
        }

        [HttpPost]
        [Route("run-job")]
        public async Task<IActionResult> RunJob([FromBody] ManageScheduleJobRequest request)
        {
            if (string.IsNullOrEmpty(request.jobName))
            {
                return BadRequest("Job name cannot be empty.");
            }

            await _jobSchedule.RunJobScheduleAsync(request.jobName);
            return Ok("Job scheduled successfully.");
        }

        [HttpPost]
        [Route("pause-job")]
        public async Task<IActionResult> PauseJob([FromBody] ManageScheduleJobRequest request)
        {
            if (string.IsNullOrEmpty(request.jobName))
            {
                return BadRequest("Job name cannot be empty.");
            }

            await _jobSchedule.PauseJobScheduleAsync(request.jobName);
            return Ok("Job scheduled successfully.");
        }
    }
}