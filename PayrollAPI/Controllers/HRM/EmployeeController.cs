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
    }
}
