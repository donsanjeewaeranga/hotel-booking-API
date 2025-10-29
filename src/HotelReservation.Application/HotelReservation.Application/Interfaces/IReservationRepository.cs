using HotelReservation.Domain.Entities;
using HotelReservation.Domain.Enums;

namespace HotelReservation.Application.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsByGuestIdAsync(int guestId);
    Task<IEnumerable<Reservation>> GetReservationsByRoomIdAsync(int roomId);
    Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(ReservationStatus status);
    Task<Reservation?> GetWithDetailsAsync(int reservationId);
    Task<bool> HasConflictingReservationAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int? excludeReservationId = null);
}
