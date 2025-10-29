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
    public async Task<ActionResult<IEnumerable<RoomDto>>> SearchRooms([FromQuery] RoomSearchDto searchDto)
    {
        try
        {
            if (searchDto.CheckInDate >= searchDto.CheckOutDate)
            {
                return BadRequest(new { message = "Check-out date must be after check-in date." });
            }

            if (searchDto.CheckInDate < DateTime.Today)
            {
                return BadRequest(new { message = "Check-in date cannot be in the past." });
            }

            var rooms = await _roomService.GetAvailableRoomsAsync(searchDto);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for rooms");
            return StatusCode(500, new { message = "An error occurred while searching for rooms." });
        }
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
