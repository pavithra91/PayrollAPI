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

        /// <summary>
        /// Method used to manage (assign, update and delete) special tax to employees
        /// </summary>
        /// <param name="specialTaxEmpDto"></param>
        /// <returns></returns>
        [Route("ManageSpecialTaxEmp")]
        [HttpPost]
        public IActionResult ManageSpecialTaxEmp([FromBody] SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = _admin.ManageSpecialTaxEmp(specialTaxEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }
        /// <summary>
        /// Method used to manage (assign, update and delete) special rate to specific paycodes
        /// </summary>
        /// <param name="specialRateEmpDto"></param>
        /// <returns></returns>
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
