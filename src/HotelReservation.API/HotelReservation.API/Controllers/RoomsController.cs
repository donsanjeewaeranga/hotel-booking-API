using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet("search")]
        public async Task<IActionResult> SearchRooms(
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            [FromQuery] int numberOfGuests)
        {
            // Validate dates
            if (checkIn.Date < DateTime.Now.Date)
            {
                return BadRequest("Check-in date must be in the future");
            }

            if (checkOut.Date <= checkIn.Date)
            {
                return BadRequest("Check-out date must be after check-in date");
            }

            // Validate number of guests
            if (numberOfGuests <= 0)
            {
                return BadRequest("Number of guests must be greater than 0");
            }

            var availableRooms = await _roomService.SearchAvailableRoomsAsync(checkIn, checkOut, numberOfGuests);
            return Ok(availableRooms);
        }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetRoom(int id)
    {
        try
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found." });
            }

            return Ok(room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room {RoomId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the room." });
        }
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetRoomTypes()
    {
        try
        {
            var roomTypes = await _roomService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room types");
            return StatusCode(500, new { message = "An error occurred while retrieving room types." });
        }
    }

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<object>> CheckAvailability(int id, [FromQuery] DateTime checkInDate, [FromQuery] DateTime checkOutDate)
    {
        try
        {
            if (checkInDate >= checkOutDate)
            {
                return BadRequest(new { message = "Check-out date must be after check-in date." });
            }

            if (checkInDate < DateTime.Today)
            {
                return BadRequest(new { message = "Check-in date cannot be in the past." });
            }

            var isAvailable = await _roomService.IsRoomAvailableAsync(id, checkInDate, checkOutDate);
            
            return Ok(new
            {
                roomId = id,
                checkInDate,
                checkOutDate,
                isAvailable
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for room {RoomId}", id);
            return StatusCode(500, new { message = "An error occurred while checking room availability." });
        }
    }
}
