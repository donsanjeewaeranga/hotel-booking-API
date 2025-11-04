using HotelReservation.Domain.Enums;

namespace HotelReservation.Application.DTOs;

public class RegisterGuestDto
{
    // User information
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // Guest information
    public string? Title { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}