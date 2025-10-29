using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Repositories;

public class RoomFeatureRepository : Repository<RoomFeature>, IRoomFeatureRepository
{
    public RoomFeatureRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RoomFeature>> GetFeaturesByRoomIdAsync(int roomId)
    {
        return await _context.RoomFeatureAssignments
            .Where(rfa => rfa.RoomId == roomId)
            .Include(rfa => rfa.RoomFeature)
            .Select(rfa => rfa.RoomFeature)
            .ToListAsync();
    }
}
