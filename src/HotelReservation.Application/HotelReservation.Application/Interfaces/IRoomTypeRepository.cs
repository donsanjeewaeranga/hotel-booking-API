using HotelReservation.Domain.Entities;

namespace HotelReservation.Application.Interfaces;

public interface IRoomTypeRepository : IRepository<RoomType>
{
    Task<IEnumerable<RoomType>> GetActiveRoomTypesAsync();
}
