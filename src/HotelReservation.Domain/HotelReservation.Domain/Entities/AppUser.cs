using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Entities;

public class AppUser
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public Guest? Guest { get; set; }
}
