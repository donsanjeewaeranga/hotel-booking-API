namespace HotelReservation.Domain.Entities;

public class RoomFeatureAssignment
{
    public int RoomId { get; set; }
    public int RoomFeatureId { get; set; }
    
    // Navigation properties
    public Room Room { get; set; } = null!;
    public RoomFeature RoomFeature { get; set; } = null!;
}
