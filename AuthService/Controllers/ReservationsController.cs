using System.Diagnostics.Contracts;
using System.Security.Claims;
using AuthService.DTOs.ReservationDto;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationsController(IReservationService service)
        {
            _reservationService = service;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound($"Reservation with Id {id} not found");
            return Ok(reservation);
        }

        [HttpGet("my-reservations")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var reservations = await _reservationService.GetMyReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var reservation = await _reservationService.CreateReservationAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{id}/fulfil")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Fulfil(int id, [FromQuery] int dueDays = 14)
        {
            try
            {
                var result = await _reservationService.FulfilReservationAsync(id, dueDays);
                if (!result) return NotFound();
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
