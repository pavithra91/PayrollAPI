using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;

namespace PayrollAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _admin;
        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }

        [Route("SpecialRateEmp")]
        [HttpPost]
        public IActionResult SpecialRateEmp([FromBody] SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = _admin.AddSpecialRateEmp(specialRateEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }
    }
}
