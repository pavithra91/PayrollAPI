using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Authentication;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces.Payroll;
using PayrollAPI.Services;

namespace PayrollAPI.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _admin;
        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }
        /// <summary>
        /// Get Tax Calculation Details
        /// </summary>
        /// <returns></returns>
        [Route("get-tax-details")]
        [HttpGet]
        public async Task<ActionResult> GetTaxDetails()
        {
            MsgDto _msg = await _admin.GetTaxDetails();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Get Tax Calculation Details By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-tax-details-id")]
        [HttpGet]
        public async Task<ActionResult> GetTaxDetailsById(int id)
        {
            MsgDto _msg = await _admin.GetTaxDetailsById(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Create New Tax Calculation
        /// </summary>
        /// <param name="taxCalDto"></param>
        /// <returns></returns>
        [Route("create-tax")]
        [HttpPost]
        public async Task<ActionResult> CreateTaxCalculation([FromBody] TaxCalDto taxCalDto)
        {
            MsgDto _msg = await _admin.CreateTaxCalculation(taxCalDto);

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
        /// Update existing Tax Calculation
        /// </summary>
        /// <param name="taxCalDto"></param>
        /// <returns></returns>
        [Route("update-tax")]
        [HttpPut]
        public async Task<ActionResult> UpdateTax([FromBody] TaxCalDto taxCalDto)
        {
            MsgDto _msg = await _admin.UpdateTax(taxCalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Get all Paycode Details
        /// </summary>
        /// <returns></returns>
        [Route("get-paycodes")]
        [HttpGet]
        public async Task<ActionResult> GetPayCodes()
        {
            MsgDto _msg = await _admin.GetPayCodes();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Get all Paycode Details By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-paycodes-id")]
        [HttpGet]
        public async Task<ActionResult> GetPayCodesById(int id)
        {
            MsgDto _msg = await _admin.GetPayCodesById(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Method used to map SAP pay code with Calculation codes
        /// </summary>
        /// <param name="payCodeDto"></param>
        /// <returns></returns>
        [Route("create-paycode")]
        [HttpPost]
        public async Task<IActionResult> CreatePayCode([FromBody] PayCodeDto payCodeDto)
        {
            MsgDto _msg = await _admin.CreatePayCode(payCodeDto);

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
        /// Update existing Paycode Mapping
        /// </summary>
        /// <param name="payCodeDto"></param>
        /// <returns></returns>
        [Route("update-paycode")]
        [HttpPut]
        public async Task<ActionResult> UpdatePayCode([FromBody] PayCodeDto payCodeDto)
        {
            MsgDto _msg = await _admin.UpdatePayCode(payCodeDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }


        /// <summary>
        /// Get all Calculations
        /// </summary>
        /// <returns></returns>
        [Route("get-calculations")]
        [HttpGet]
        public async Task<ActionResult> GetCalculations()
        {
            MsgDto _msg = await _admin.GetCalculations();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Get all Calculations By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-calculations-id")]
        [HttpGet]
        public async Task<ActionResult> GetCalculationsById(int id)
        {
            MsgDto _msg = await _admin.GetCalculationsById(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Create new payroll calculations
        /// </summary>
        /// <param name="calDto"></param>
        /// <returns></returns>
        [Route("create-calculation")]
        [HttpPost]
        public async Task<ActionResult> CreateCalculation([FromBody] CalDto calDto)
        {
            MsgDto _msg = await _admin.CreateCalculation(calDto);

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
        /// Update existing Payroll Calculation
        /// </summary>
        /// <param name="calDto"></param>
        /// <returns></returns>
        [Route("update-calculation")]
        [HttpPut]
        public async Task<ActionResult> UpdateCalculation([FromBody] CalDto calDto)
        {
            MsgDto _msg = await _admin.UpdateCalculation(calDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Delete existing Payroll Calculation (Mark for Deletion)
        /// </summary>
        /// <param name="calDto"></param>
        /// <returns></returns>
        [Route("delete-calculation")]
        [HttpPut]
        public async Task<ActionResult> DeleteCalculation([FromBody] CalDto calDto)
        {
            MsgDto _msg = await _admin.DeleteCalculation(calDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Get all Special rate employees
        /// </summary>
        /// <returns></returns>
        [Route("get-empsplrate")]
        [HttpGet]
        public async Task<ActionResult> GetSplRateEmp()
        {
            MsgDto _msg = await _admin.GetSplRateEmp();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Get all Special rate employees By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-empsplrate-id")]
        [HttpGet]
        public async Task<ActionResult> GetSplRateEmpById(int id)
        {
            MsgDto _msg = await _admin.GetSplRateEmpById(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Method used to manage assign special rate to specific paycodes
        /// </summary>
        /// <param name="specialRateEmpDto"></param>
        /// <returns></returns>
        [Route("create-special-rate")]
        [HttpPost]
        public async Task<ActionResult> CreateSpecialRateEmp([FromBody] SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = await _admin.CreateSpecialRateEmp(specialRateEmpDto);

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
        /// Update existing special rate assign to Employee
        /// </summary>
        /// <param name="specialRateEmpDto"></param>
        /// <returns></returns>
        [Route("update-special-rate")]
        [HttpPut]
        public async Task<ActionResult> UpdateSpecialRateEmp([FromBody] SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = await _admin.UpdateSpecialRateEmp(specialRateEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Delete existing special rate assign to Employee (Mark for Deletion)
        /// </summary>
        /// <param name="specialRateEmpDto"></param>
        /// <returns></returns>
        [Route("delete-special-rate")]
        [HttpPut]
        public async Task<ActionResult> DeleteSpecialRateEmp([FromBody] SpecialRateEmpDto specialRateEmpDto)
        {
            MsgDto _msg = await _admin.DeleteSpecialRateEmp(specialRateEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Get all Special Tax assign to Employees
        /// </summary>
        /// <returns></returns>
        [Route("get-empspltax")]
        [HttpGet]
        public async Task<ActionResult> GetSplTaxEmp()
        {
            MsgDto _msg = await _admin.GetSplTaxEmp();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Get all Special Tax assign to Employee By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-empspltax-id")]
        [HttpGet]
        public async Task<ActionResult> GetSplTaxEmpById(int id)
        {
            MsgDto _msg = await _admin.GetSplTaxEmpById(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        /// <summary>
        /// Method used to assign special tax to employees
        /// </summary>
        /// <param name="specialTaxEmpDto"></param>
        /// <returns></returns>
        [Route("create-special-tax")]
        [HttpPost]
        public async Task<ActionResult> CreateSpecialTaxEmp([FromBody] SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = await _admin.CreateSpecialTaxEmp(specialTaxEmpDto);

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
        /// Update existing special tax assign to Employee
        /// </summary>
        /// <param name="specialTaxEmpDto"></param>
        /// <returns></returns>
        [Route("update-special-tax")]
        [HttpPut]
        public async Task<ActionResult> UpdateSpecialTaxEmp([FromBody] SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = await _admin.UpdateSpecialTaxEmp(specialTaxEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        /// <summary>
        /// Delete existing special tax assign to Employee (Mark for Deletion)
        /// </summary>
        /// <param name="specialTaxEmpDto"></param>
        /// <returns></returns>
        [Route("delete-special-tax")]
        [HttpPut]
        public async Task<ActionResult> DeleteSpecialTaxEmp([FromBody] SpecialTaxEmpDto specialTaxEmpDto)
        {
            MsgDto _msg = await _admin.DeleteSpecialTaxEmp(specialTaxEmpDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("get-ot-details")]
        [HttpGet]
        public async Task<ActionResult> GetOTDetails(int period, int companyCode)
        {
            MsgDto _msg = await _admin.GetOTDetails(period, companyCode);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("get-unrecovered-details")]
        [HttpGet]
        public async Task<ActionResult> GetUnrecoveredDetails(int period, int companyCode)
        {
            MsgDto _msg = await _admin.GetUnrecoveredDetails(period, companyCode);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("get-lumpsumtax-details")]
        [HttpGet]
        public async Task<ActionResult> GetLumpSumTaxDetails(int period, int companyCode)
        {
            MsgDto _msg = await _admin.GetLumpSumTaxDetails(period, companyCode);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("reset-data")]
        [HttpPost]
        public async Task<ActionResult> ResetData(ResetDto resetDto)
        {
            MsgDto _msg = await _admin.ResetData(resetDto);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("get-system-variables")]
        [HttpGet]
        public async Task<ActionResult> GetSystemVariables()
        {
            MsgDto _msg = await _admin.GetSystemVariables();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("create-system-variable")]
        [HttpPost]
        public async Task<ActionResult> CreateSystemVariable([FromBody] SysVariableDto sysVariableDto)
        {
            MsgDto _msg = await _admin.CreateSystemVariable(sysVariableDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("update-system-variable")]
        [HttpPut]
        public async Task<ActionResult> UpdateSystemVariable([FromBody] SysVariableDto sysVariableDto)
        {
            MsgDto _msg = await _admin.UpdateSystemVariable(sysVariableDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("get-paycodewise-details")]
        [HttpGet]
        public async Task<ActionResult> GetPayCodeWiseData(int period, int companyCode)
        {
            MsgDto _msg = await _admin.GetPayCodeWiseData(period, companyCode);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        //// Just for checking
        //[Route("checklogger")]
        //[HttpGet]
        //public async Task<ActionResult> CheckLogger()
        //{
        //    try
        //    {      
        //        return Ok();
        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
