using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [Route("ConfirmDataTransfer")]
        [HttpPost]
        public IActionResult ConfirmDataTransfer()
        {
            MsgDto _msg = _data.ConfirmDataTransfer(02);


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
