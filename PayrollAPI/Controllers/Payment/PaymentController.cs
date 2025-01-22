using DocumentFormat.OpenXml.Office2010.Excel;
using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Authentication;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces.Payment;
using PayrollAPI.Interfaces.Reservation;
using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment _payment;
        public PaymentController(IPayment payment)
        {
            _payment = payment;
        }

        [HttpGet("get-voucher/{id}")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVoucherPayments([FromRoute] string id)
        {
            string decodedId = System.Web.HttpUtility.UrlDecode(id);

            var result = await _payment.GetVoucherPayments(decodedId);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("process-voucher")]
        public async Task<IActionResult> ProcessVoucher([FromBody] PaymentRequest request)
        {
            var result = await _payment.ProcessVoucher(request.voucherNo, request.bankDate, request.processBy);

            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("reset-voucher")]
        public async Task<IActionResult> ResetVoucher([FromBody] PaymentResetRequest request)
        {
            var result = await _payment.ResetVoucher(request.voucherNo, request.lastUpdateBy);

            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
