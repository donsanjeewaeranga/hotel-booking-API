using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Entities;

public class Room
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public RoomType RoomType { get; set; } = null!;
    public ICollection<RoomFeatureAssignment> RoomFeatureAssignments { get; set; } = new List<RoomFeatureAssignment>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
