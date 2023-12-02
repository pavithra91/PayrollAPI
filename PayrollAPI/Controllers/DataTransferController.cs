using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;

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

        /// <summary>
        /// Method used to Confirm the data transfer is Complete
        /// </summary>
        /// <param name="approvalDto"></param>
        /// <returns></returns>
        [Route("ConfirmDataTransfer")]
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
    }
}
