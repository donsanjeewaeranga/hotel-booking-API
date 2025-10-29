using HotelReservation.Application.Interfaces;
using HotelReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelReservation.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        AppUsers = new AppUserRepository(_context);
        Guests = new GuestRepository(_context);
        Rooms = new RoomRepository(_context);
        RoomTypes = new RoomTypeRepository(_context);
        RoomFeatures = new RoomFeatureRepository(_context);
        Reservations = new ReservationRepository(_context);
    }

    public IAppUserRepository AppUsers { get; }
    public IGuestRepository Guests { get; }
    public IRoomRepository Rooms { get; }
    public IRoomTypeRepository RoomTypes { get; }
    public IRoomFeatureRepository RoomFeatures { get; }
    public IReservationRepository Reservations { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
