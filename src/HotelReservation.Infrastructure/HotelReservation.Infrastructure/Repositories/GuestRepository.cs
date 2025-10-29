using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Repositories;

public class GuestRepository : Repository<Guest>, IGuestRepository
{
    public GuestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Guest?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g => g.UserId == userId);
    }

    public async Task<Guest?> GetWithUserAsync(int guestId)
    {
        return await _dbSet
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.GuestId == guestId);
    }
}
