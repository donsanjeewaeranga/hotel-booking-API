using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using HotelReservation.Domain.Enums;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int? roomTypeId = null)
    {
        var query = _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.RoomFeatureAssignments)
                .ThenInclude(rfa => rfa.RoomFeature)
            .Where(r => r.Status == RoomStatus.Available);

        if (roomTypeId.HasValue)
        {
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
        }

        // Check for conflicting reservations
        var conflictingRoomIds = await _context.Reservations
            .Where(r => r.ReservationStatus == ReservationStatus.Confirmed &&
                       r.CheckInDate < checkOutDate &&
                       r.CheckOutDate > checkInDate)
            .Select(r => r.RoomId)
            .ToListAsync();

        query = query.Where(r => !conflictingRoomIds.Contains(r.RoomId));

        return await query.ToListAsync();
    }

    public async Task<Room?> GetWithDetailsAsync(int roomId)
    {
        return await _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.RoomFeatureAssignments)
                .ThenInclude(rfa => rfa.RoomFeature)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
    }

    public async Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId)
    {
        return await _dbSet
            .Include(r => r.RoomType)
            .Include(r => r.RoomFeatureAssignments)
                .ThenInclude(rfa => rfa.RoomFeature)
            .Where(r => r.RoomTypeId == roomTypeId)
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        var room = await _dbSet.FindAsync(roomId);
        if (room == null || room.Status != RoomStatus.Available)
            return false;

        var hasConflict = await _context.Reservations
            .AnyAsync(r => r.RoomId == roomId &&
                          r.ReservationStatus == ReservationStatus.Confirmed &&
                          r.CheckInDate < checkOutDate &&
                          r.CheckOutDate > checkInDate);

        return !hasConflict;
    }
}
