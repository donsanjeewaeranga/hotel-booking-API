using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(RoomSearchDto searchDto);
    Task<RoomDto?> GetRoomByIdAsync(int roomId);
    Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync();
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);
}
