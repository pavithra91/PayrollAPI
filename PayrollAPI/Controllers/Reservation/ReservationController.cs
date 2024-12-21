using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Interfaces.Reservation;

namespace PayrollAPI.Controllers.Reservation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservation _reservation;
        public ReservationController(IReservation reservation)
        {
            _reservation = reservation;
        }

        [HttpGet]
        [Route("get-all-bungalows")]
        [ProducesResponseType(typeof(IEnumerable<BungalowResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBungalows()
        {
            var result = await _reservation.GetAllBungalows();

            var _bungalowResponse = result.MapToResponse();
            return Ok(_bungalowResponse);
        }

        [HttpGet("get-bungalow/{id:int}")]
        [ProducesResponseType(typeof(BungalowResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBungalowById([FromRoute] int id)
        {
            var result = await _reservation.GetBungalowById(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("create-bungalow")]
        public async Task<IActionResult> CreateBungalow([FromBody] BungalowRequest bungalow)
        {
            var _bungalow = bungalow.MapToBungalow();
            await _reservation.CreateBungalow(_bungalow);
            return CreatedAtAction(nameof(GetBungalowById), new { id = _bungalow.id }, _bungalow);
        }

        [HttpPut("update-bungalow/{id:int}")]
        public async Task<IActionResult> UpdateBungalow([FromRoute] int id, [FromBody] UpdateBungalowRequest request)
        {
            var bungalow = request.MapToBungalow(id);
            var updated = await _reservation.UpdateBungalow(id, bungalow);

            if (!updated)
            {
                return NotFound();
            }
            var response = bungalow.MapToResponse();
            return Ok(response);
        }
    }
}
