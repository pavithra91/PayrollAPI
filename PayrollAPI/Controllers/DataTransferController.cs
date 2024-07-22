using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;

namespace PayrollAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTransferController : ControllerBase
    {
        private readonly IDatatransfer _data;
        public DataTransferController(IDatatransfer data)
        {
            _data = data;
        }
        /// <summary>
        /// Method used to get data from SAP Payroll Data Extractor
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("DataTransfer")]
        [HttpPost]
        public async Task<IActionResult> DataTransfer([FromBody] object json)
        {
            MsgDto _msg = await _data.DataTransfer(json.ToString());


            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }


        [Route("paycode-check")]
        [HttpGet]
        public async Task<IActionResult> PayCodeCheck(int companyCode, int period)
        {
            MsgDto _msg = await _data.PayCodeCheck(companyCode, period);

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
        /// 
        /// </summary>
        /// <param name="approvalDto"></param>
        /// <returns></returns>
        [Route("temp-data-rollback")]
        [HttpPost]
        public async Task<IActionResult> RollBackTempData([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _data.RollBackTempData(approvalDto);


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
        /// Method used to Confirm the data transfer is Complete
        /// </summary>
        /// <param name="approvalDto"></param>
        /// <returns></returns>
        [Route("confirm-data-transfer")]
        [HttpPost]
        public async Task<IActionResult> ConfirmDataTransfer([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _data.ConfirmDataTransfer(approvalDto);


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
        /// Method used to approve payroll data. (Copy data to transaction tables)
        /// </summary>
        /// <param name="approvalDto"></param>
        /// <returns></returns>
        [Route("PreparePayrun")]
        [HttpPost]
        public async Task<IActionResult> PreparePayrun([FromBody] ApprovalDto approvalDto)
        {
            MsgDto _msg = await _data.PreparePayrun(approvalDto);


            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("GetEmployeeList")]
        [HttpGet]
        public IActionResult GetEmployeeList(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();
            if (companyCode < 0 || period < 0)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Please enter Company Code & Period";
                return BadRequest(_msg);
            }
            ICollection<Temp_Employee> _empList = _data.GetTempEmployeeList(companyCode, period);

            if (_empList != null)
                return Ok(_empList);
            else
                return NotFound();
        }

        [Route("GetDataTransferStatistics")]
        [HttpGet]
        public IActionResult GetDataTransferStatistics(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();
            if (companyCode < 0 || period < 0)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Please enter Company Code & Period";
                return BadRequest(_msg);
            }
            _msg = _data.GetDataTransferStatistics(companyCode, period);

            if (_msg.MsgCode != 'E')
                return Ok(_msg);
            else
                return NotFound(_msg);
        }
    }
}
