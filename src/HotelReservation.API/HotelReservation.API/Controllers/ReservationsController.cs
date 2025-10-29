using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationDto createReservationDto)
    {
        try
        {
            var validator = new CreateReservationDtoValidator();
            var validationResult = await validator.ValidateAsync(createReservationDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var reservation = await _reservationService.CreateReservationAsync(createReservationDto);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.ReservationId }, reservation);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found during reservation creation");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during reservation creation");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during reservation creation");
            return StatusCode(500, new { message = "An unexpected error occurred while creating the reservation." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        try
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found." });
            }

            return Ok(reservation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservation {ReservationId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the reservation." });
        }
    }

    [HttpGet("guest/{guestId}")]
    public async Task<ActionResult<IEnumerable<ReservationSummaryDto>>> GetReservationsByGuest(int guestId)
    {
        try
        {
            var reservations = await _reservationService.GetReservationsByGuestIdAsync(guestId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations for guest {GuestId}", guestId);
            return StatusCode(500, new { message = "An error occurred while retrieving reservations." });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<object>> CancelReservation(int id)
    {
        try
        {
            var success = await _reservationService.CancelReservationAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Reservation not found or cannot be canceled." });
            }

            return Ok(new { message = "Reservation canceled successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling reservation {ReservationId}", id);
            return StatusCode(500, new { message = "An error occurred while canceling the reservation." });
        }
    }

    [HttpPut("{id}/checkin")]
    public async Task<ActionResult<object>> CheckInReservation(int id)
    {
        try
        {
            var success = await _reservationService.CheckInReservationAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Reservation not found or cannot be checked in." });
            }

            return Ok(new { message = "Reservation checked in successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in reservation {ReservationId}", id);
            return StatusCode(500, new { message = "An error occurred while checking in the reservation." });
        }
    }

    [HttpPut("{id}/checkout")]
    public async Task<ActionResult<object>> CheckOutReservation(int id)
    {
        try
        {
            var success = await _reservationService.CheckOutReservationAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Reservation not found or cannot be checked out." });
            }

            return Ok(new { message = "Reservation checked out successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out reservation {ReservationId}", id);
            return StatusCode(500, new { message = "An error occurred while checking out the reservation." });
        }
    }
}
