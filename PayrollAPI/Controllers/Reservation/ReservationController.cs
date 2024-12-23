using DocumentFormat.OpenXml.Office2010.Excel;
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
        private readonly IEmployee _employee;
        public ReservationController(IReservation reservation, IEmployee employee)
        {
            _reservation = reservation;
            _employee = employee;
        }

        #region Bungalow
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

        #endregion

        #region Reservation
        [HttpGet]
        [Route("get-all-reservations")]
        [ProducesResponseType(typeof(IEnumerable<ReservationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllReservations()
        {
            var result = await _reservation.GetAllReservations();

            var _reservationResponse = result.MapToResponse();
            return Ok(_reservationResponse);
        }

        [HttpGet("get-reservation/{id:int}")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReservationById([FromRoute] int id)
        {
            var result = await _reservation.GetReservationById(id);

            return result == null ? NotFound() :
                Ok(result.MapToResponse());

        }

        [HttpPost]
        [Route("create-reservation")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request)
        {
            var emp = await _employee.GetEmployeeByEPF(request.epf);
            var bungalow = await _reservation.GetBungalowById(request.bungalowid);
            var category = await _reservation.GetReservationCategoryById(request.category);

            var _reservationResponse = request.MapToReservation(emp, bungalow, category);
            await _reservation.CreateReservation(_reservationResponse);

            return CreatedAtAction(nameof(GetReservationById), new { id = _reservationResponse.id }, _reservationResponse);
        }

        [HttpPut("update-reservation/{id:int}")]
        public async Task<IActionResult> UpdateReservation([FromRoute] int id, [FromBody] UpdateReservationRequest request)
        {
            var emp = await _employee.GetEmployeeByEPF(request.epf);
            var bungalow = await _reservation.GetBungalowById(request.bungalowid);

            var reservation = request.MapToReservation(id, emp, bungalow);
            var updated = await _reservation.UpdateReservation(id, reservation);

            if (!updated)
            {
                return NotFound();
            }
            var response = reservation.MapToResponse();
            return Ok(response);
        }
        #endregion
    }
}
