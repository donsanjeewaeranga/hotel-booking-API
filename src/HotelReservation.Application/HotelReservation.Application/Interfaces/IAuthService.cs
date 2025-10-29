using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
    Task<bool> ValidatePasswordAsync(string email, string password);
    Task<string> GenerateJwtTokenAsync(UserDto user);
}
