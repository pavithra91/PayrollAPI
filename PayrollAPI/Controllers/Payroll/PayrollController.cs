using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Authentication;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces.Payroll;

namespace PayrollAPI.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class PayrollController : ControllerBase
    {
        private readonly IPayroll _payroll;
        public PayrollController(IPayroll payroll)
        {
            _payroll = payroll;
        }


        [Route("simulate-payroll")]
        [HttpPost]
        public async Task<IActionResult> SimulatePayroll(ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.SimulatePayroll(approvalDto);

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
        /// Method used to start the payroll process
        /// </summary>
        /// <param name="approvalDto"></param>
        /// <returns></returns>
        [Route("process-payroll")]
        [HttpPost]
        public async Task<IActionResult> ProcessPayroll([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.ProcessPayroll(approvalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("ProcessPayrollbyEPF")]
        [HttpPost]
        public async Task<ActionResult> ProcessPayrollbyEPF(string epf)
        {
            int period = 202310;
            int companyCode = 3000;

            MsgDto _msg = await _payroll.ProcessPayrollbyEPF(epf, period, companyCode);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("create-unrecovered")]
        [HttpPost]
        public async Task<ActionResult> CreateUnrecoveredFile([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.CreateUnrecoveredFile(approvalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return Ok(_msg);
            }
        }

        [Route("get-payroll-summary")]
        [HttpGet]
        public async Task<ActionResult> GetPayrollSummary(int period, int companyCode)
        {
            MsgDto _msg = await _payroll.GetPayrollSummary(period, companyCode);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return NoContent();
            }
        }

        [Route("get-paysheet")]
        [HttpGet]
        public async Task<ActionResult> GetPaySheet(string epf, int period)
        {
            MsgDto _msg = await _payroll.GetPaySheet(epf, period);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("print-paysheet")]
        [HttpGet]
        public async Task<ActionResult> PrintPaySheets(int companyCode, int period)
        {
            MsgDto _msg = await _payroll.PrintPaySheets(companyCode, period);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("get-payrun")]
        [HttpGet]
        public async Task<ActionResult> GetPayrunDetails()
        {
            MsgDto _msg = await _payroll.GetPayrunDetails();

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("get-payrun-by-period")]
        [HttpGet]
        public async Task<ActionResult> GetPayrunDetails(int period, int companyCode)
        {
            MsgDto _msg = await _payroll.GetPayrunDetails(period, companyCode);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("write-back")]
        [HttpGet]
        public async Task<ActionResult> Writeback(int period, int companyCode)
        {
            MsgDto _msg = await _payroll.Writeback(period, companyCode);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("stop-salary")]
        [HttpPost]
        public async Task<ActionResult> StopSalary([FromBody] StopSalDto stopSalDto)
        {
            MsgDto _msg = await _payroll.StopSalary(stopSalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("create-bank-file")]
        [HttpPost]
        public async Task<ActionResult> CreateBankFile([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.CreateBankFile(approvalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("get-emp-paysheet")]
        [HttpPost]
        public async Task<ActionResult> GetEmployeePaySheet([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.GetEmployeePaySheet(approvalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("resend-paysheets")]
        [HttpPost]
        public async Task<ActionResult> ResendPaySheets([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _payroll.ResendPaySheets(approvalDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        // Just for checking
        [AllowAnonymous]
        [Route("checklogger")]
        [HttpGet]
        public async Task<ActionResult> CheckLogger()
        {
            try
            {
                _payroll.CheckLogger();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
