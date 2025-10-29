using HotelReservation.Domain.Entities;

namespace HotelReservation.Application.Interfaces;

public interface IRoomFeatureRepository : IRepository<RoomFeature>
{
    Task<IEnumerable<RoomFeature>> GetFeaturesByRoomIdAsync(int roomId);
}
