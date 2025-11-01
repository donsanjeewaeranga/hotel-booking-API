using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> SearchRoomsAsync(string? searchTerm);
        Task<IEnumerable<RoomDto>> SearchAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int numberOfGuests);
    Task<RoomDto?> GetRoomByIdAsync(int roomId);
    Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync();
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);
}
