using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Repositories;

public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RoomType>> GetActiveRoomTypesAsync()
    {
        return await _dbSet
            .Where(rt => rt.DeletedAt == null)
            .ToListAsync();
    }
}
