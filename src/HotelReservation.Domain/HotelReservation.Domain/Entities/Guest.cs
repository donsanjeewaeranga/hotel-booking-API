namespace HotelReservation.Domain.Entities;

public class Guest
{
    public int GuestId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
