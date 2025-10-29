using HotelReservation.Domain.Entities;

namespace HotelReservation.Application.Interfaces;

public interface IGuestRepository : IRepository<Guest>
{
    Task<Guest?> GetByUserIdAsync(int userId);
    Task<Guest?> GetWithUserAsync(int guestId);
}
