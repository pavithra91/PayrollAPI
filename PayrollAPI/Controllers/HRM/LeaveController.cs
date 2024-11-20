using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LeaveController : ControllerBase
    {
        private readonly ILeave _leave;
        public LeaveController(ILeave leave)
        {
            _leave = leave;
        }

        [HttpGet]
        [Route("get-all-leaveTypes")]
        [ProducesResponseType(typeof(IEnumerable<LeaveTypeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            var result = await _leave.GetAllLeaveTypes();

            var _leaveTypeResponse = result.MapToResponse();
            return Ok(_leaveTypeResponse);
        }

        [HttpGet("get-leaveType/{id:int}")]
        [ProducesResponseType(typeof(LeaveTypeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLeaveType([FromRoute] int id)
        {
            var result = await _leave.GetLeaveType(id);

            return result == null ? NotFound() : 
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("create-leaveType")]
        public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeRequest request)
        {
            var leaveType = request.MapToLeaveType();
            await _leave.CreateLeaveType(leaveType);
            return CreatedAtAction(nameof(GetLeaveType), new { id = leaveType.leaveTypeId }, leaveType);
        }

        [HttpPut("update-leaveType/{id:int}")]
        public async Task<IActionResult> UpdateLeaveType([FromRoute] int id, [FromBody] UpdateLeaveTypeRequest request)
        {
            var leaveType = request.MapToLeaveType(id);
            var updated = await _leave.UpdateLeaveType(id, leaveType);
            
            if(!updated)
            {
                return NotFound();
            }
            var response = leaveType.MapToResponse();
            return Ok(response);
        }

        [HttpGet]
        [Route("get-all-supervisors")]
        [ProducesResponseType(typeof(IEnumerable<SupervisorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSupervisors()
        {
            var result = await _leave.GetAllSupervisors();

            var _supervisorResponse = result.MapToResponse();
            return Ok(_supervisorResponse);
        }

        [HttpGet("get-supervisor/{id:int}")]
        [ProducesResponseType(typeof(SupervisorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSupervisors([FromRoute] int id)
        {
            var result = await _leave.GetSupervisor(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("create-supervisor")]
        public async Task<IActionResult> CreateSupervisor([FromBody] SupervisorRequest request)
        {
            var supervisor = request.MapToSupervisor();
            await _leave.CreateSupervisor(supervisor);
            return CreatedAtAction(nameof(GetSupervisors), new { id = supervisor.id }, supervisor);
        }

        [HttpPut("update-supervisor/{id:int}")]
        public async Task<IActionResult> UpdateSupervisor([FromRoute] int id, [FromBody] UpdateSupervisorRequest request)
        {
            var supervisor = request.MapToSupervisor(id);
            var updated = await _leave.UpdateSupervisor(id, supervisor);

            if (!updated)
            {
                return NotFound();
            }
            var response = supervisor.MapToResponse();
            return Ok(response);
        }


        [HttpGet]
        [Route("get-all-workflow")]
        //[ProducesResponseType(typeof(IEnumerable<LeaveTypeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApprovalWorkflow()
        {
            var result = await _leave.GetApprovalWorkflow();
            return Ok(result);
        }

        [HttpPost]
        [Route("assign-supervisor")]
        public async Task<IActionResult> AssignSupervisor([FromBody] AssignSupervisorRequest request)
        {
            //var supervisor = request.MapToSupervisor();
            var result = await _leave.AssignSupervisor(request.epf, request.approvalLevel, request.approverNames, request.updateBy);
            if(result)
            {
                return Ok("success");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("request-leave")]
        public async Task<IActionResult> RequestLeave([FromBody] RequestLeaveRequest request)
        {
            //var supervisor = request.MapToSupervisor();
            var result = await _leave.RequestLeave(request);
            if (result)
            {
                return Ok("success");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("approve-leave")]
        public async Task<IActionResult> ApproveLeave([FromBody] ApproveLeaveRequest request)
        {
            //var supervisor = request.MapToSupervisor();
            var result = await _leave.ApproveLeave(request);
            if (result)
            {
                return Ok("success");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("get-leaveRequest/{id:int}")]
        [ProducesResponseType(typeof(LeaveRequestApprovalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLeaveRequest([FromRoute] int id)
        {
            var leaveApprovals = await _leave.GetLeaveApprovals(id);
            var result = await _leave.GetLeaveRequest(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse(leaveApprovals));

        }

        [HttpGet("get-leaveHistory/{epf:int}")]
        [ProducesResponseType(typeof(LeaveHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLeaveRequestHistory([FromRoute] int epf)
        {
            var result = await _leave.GetLeaveRequestHistory(epf);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpGet("get-dashboard/{epf:int}")]
        [ProducesResponseType(typeof(LeaveDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeaveDashboard([FromRoute] int epf)
        {
            var _leaveHistory = await _leave.GetLeaveRequestHistory(epf);
            var _leaveBalance = await _leave.GetLeaveBalance(epf);

            return _leaveBalance == null ? NotFound() :
                Ok(_leaveHistory.MapToResponse(_leaveBalance));

        }

        [HttpGet("get-notifications/{epf:int}")]
        [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotifications([FromRoute] int epf)
        {
            var _notificationList = await _leave.GetNotifications(epf);

            return _notificationList == null ? NotFound() :
               Ok(_notificationList.MapToResponse());

        }

        [HttpPost]
        [Route("request-advancePayment")]
        public async Task<IActionResult> RequestAdvancePayment([FromBody] AdvancePaymentRequest request)
        {
            var advancePayment = request.MapToAdvancePayment();
            var result = await _leave.RequestAdvancePayment(advancePayment);
            if (result)
            {
                return Ok("success");
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
