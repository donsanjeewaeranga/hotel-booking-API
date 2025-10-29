using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IGuestService
{
    Task<GuestDto?> GetGuestByUserIdAsync(int userId);
    Task<GuestDto> CreateGuestAsync(CreateGuestDto createGuestDto);
    Task<GuestDto> UpdateGuestAsync(int guestId, UpdateGuestDto updateGuestDto);
    Task<bool> DeleteGuestAsync(int guestId);
}
