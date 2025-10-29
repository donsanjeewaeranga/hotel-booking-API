namespace HotelReservation.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAppUserRepository AppUsers { get; }
    IGuestRepository Guests { get; }
    IRoomRepository Rooms { get; }
    IRoomTypeRepository RoomTypes { get; }
    IRoomFeatureRepository RoomFeatures { get; }
    IReservationRepository Reservations { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
