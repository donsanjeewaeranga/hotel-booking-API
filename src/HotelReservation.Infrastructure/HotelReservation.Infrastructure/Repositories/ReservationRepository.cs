using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using HotelReservation.Domain.Enums;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByGuestIdAsync(int guestId)
    {
        return await _dbSet
            .Include(r => r.Room)
                .ThenInclude(room => room.RoomType)
            .Include(r => r.Guest)
            .Where(r => r.GuestId == guestId)
            .OrderByDescending(r => r.CheckInDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByRoomIdAsync(int roomId)
    {
        return await _dbSet
            .Include(r => r.Room)
                .ThenInclude(room => room.RoomType)
            .Include(r => r.Guest)
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.CheckInDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(ReservationStatus status)
    {
        return await _dbSet
            .Include(r => r.Room)
                .ThenInclude(room => room.RoomType)
            .Include(r => r.Guest)
            .Where(r => r.ReservationStatus == status)
            .OrderByDescending(r => r.CheckInDate)
            .ToListAsync();
    }

    public async Task<Reservation?> GetWithDetailsAsync(int reservationId)
    {
        return await _dbSet
            .Include(r => r.Room)
                .ThenInclude(room => room.RoomType)
            .Include(r => r.Room)
                .ThenInclude(room => room.RoomFeatureAssignments)
                    .ThenInclude(rfa => rfa.RoomFeature)
            .Include(r => r.Guest)
                .ThenInclude(guest => guest.User)
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
    }

    public async Task<bool> HasConflictingReservationAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.RoomId == roomId &&
                       r.ReservationStatus == ReservationStatus.Confirmed &&
                       r.CheckInDate < checkOutDate &&
                       r.CheckOutDate > checkInDate);

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.ReservationId != excludeReservationId.Value);
        }

        return await query.AnyAsync();
    }
}
