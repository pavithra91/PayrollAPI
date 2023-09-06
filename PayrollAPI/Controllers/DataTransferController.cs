using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            
            if (_data.ConfirmDataTransfer())
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
