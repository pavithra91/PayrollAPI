using Leave.Contracts.Response;
using Microsoft.AspNetCore.Http;
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
    }
}
