using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces.HRM;

namespace PayrollAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employee;
        public EmployeeController(IEmployee employee)
        {
            _employee = employee;
        }

        [HttpGet]
        [Route("get-all-employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _employee.GetAllEmployees();

            var _employeeResponse = result.MapToResponse();
            return Ok(_employeeResponse);
        }

        [HttpGet]
        [Route("get-employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await _employee.GetEmployees();

            var _employeeResponse = result.MapToResponse();
            return Ok(_employeeResponse);
        }

        [HttpGet("get-employee/{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeesById([FromRoute] int id)
        {
            var result = await _employee.GetEmployeeById(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpGet("get-employee-grade/{epf}")]
        [ProducesResponseType(typeof(EmployeesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeesById([FromRoute] string epf)
        {
            var result = await _employee.GetEmployeesByGrade(epf);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpGet("get-supervisor/{id:int}")]
        [ProducesResponseType(typeof(SupervisorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSupervisors([FromRoute] int id)
        {
            var result = await _employee.GetSupervisor(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("create-supervisor")]
        public async Task<IActionResult> CreateSupervisor([FromBody] SupervisorRequest request)
        {
            var existingSupervisor = _employee.GetSupervisorByEPF(request.epf);
            if (existingSupervisor == null)
            {
                return Conflict("Supervisor Already Exsits");
            }

            var emp = await _employee.GetEmployeeByEPF(request.epf);
            var supervisor = request.MapToSupervisor(emp);
            await _employee.CreateSupervisor(supervisor);
            return CreatedAtAction(nameof(GetSupervisors), new { id = supervisor.id }, supervisor);
        }

        [HttpPut("update-supervisor/{id:int}")]
        public async Task<IActionResult> UpdateSupervisor([FromRoute] int id, [FromBody] UpdateSupervisorRequest request)
        {
            var supervisor = request.MapToSupervisor(id);
            var updated = await _employee.UpdateSupervisor(id, supervisor);

            if (!updated)
            {
                return NotFound();
            }
            //var response = supervisor.MapToResponse();
            return Ok();
        }

        [HttpPost]
        [Route("request-advancePayment")]
        public async Task<IActionResult> RequestAdvancePayment([FromBody] AdvancePaymentRequest request)
        {
            var emp = await _employee.GetEmployeeByEPF(request.epf);
            var advancePayment = request.MapToAdvancePayment(emp);
            var result = await _employee.RequestAdvancePayment(advancePayment);
            if (result)
            {
                return Ok("success");
            }
            else
            {
                return BadRequest("Advance Payment Request already send for processing");
            }
        }
    }
}
