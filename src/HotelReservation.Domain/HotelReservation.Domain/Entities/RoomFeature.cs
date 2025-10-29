namespace HotelReservation.Domain.Entities;

public class RoomFeature
{
    public int RoomFeatureId { get; set; }
    public string FeatureName { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<RoomFeatureAssignment> RoomFeatureAssignments { get; set; } = new List<RoomFeatureAssignment>();
}
