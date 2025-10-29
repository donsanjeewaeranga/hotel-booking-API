using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;
    private readonly ILogger<GuestsController> _logger;

    public GuestsController(IGuestService guestService, ILogger<GuestsController> logger)
    {
        _guestService = guestService;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<GuestDto>> GetGuestByUserId(int userId)
    {
        try
        {
            var guest = await _guestService.GetGuestByUserIdAsync(userId);
            if (guest == null)
            {
                return NotFound(new { message = "Guest not found." });
            }

            return Ok(guest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving guest for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving the guest." });
        }
    }

    [HttpPost]
    public async Task<ActionResult<GuestDto>> CreateGuest([FromBody] CreateGuestDto createGuestDto)
    {
        try
        {
            var guest = await _guestService.CreateGuestAsync(createGuestDto);
            return CreatedAtAction(nameof(GetGuestByUserId), new { userId = guest.UserId }, guest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating guest");
            return StatusCode(500, new { message = "An error occurred while creating the guest." });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GuestDto>> UpdateGuest(int id, [FromBody] UpdateGuestDto updateGuestDto)
    {
        try
        {
            var guest = await _guestService.UpdateGuestAsync(id, updateGuestDto);
            return Ok(guest);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Guest not found for update {GuestId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating guest {GuestId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the guest." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<object>> DeleteGuest(int id)
    {
        try
        {
            var success = await _guestService.DeleteGuestAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Guest not found." });
            }

            return Ok(new { message = "Guest deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting guest {GuestId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the guest." });
        }
    }
}
