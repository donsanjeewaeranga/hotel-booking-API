using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelReservation.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppUserRepository _userRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IAppUserRepository userRepository,
        IGuestRepository guestRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _guestRepository = guestRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto createUserDto)
    {
        if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            throw new InvalidOperationException("Email already exists");

        var user = _mapper.Map<AppUser>(createUserDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<(UserDto User, GuestDto Guest)> RegisterGuestAsync(RegisterGuestDto registerGuestDto)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(registerGuestDto.Email))
            throw new InvalidOperationException("Email already exists");

        // Create and save user
        var user = new AppUser
        {
            Email = registerGuestDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerGuestDto.Password),
            UserType = Domain.Enums.UserType.Guest
        };
        
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(); // Save to get the UserId

        // Create and save guest profile
        var guest = new Guest
        {
            UserId = user.UserId,
            Title = registerGuestDto.Title,
            FirstName = registerGuestDto.FirstName,
            LastName = registerGuestDto.LastName,
            PhoneNumber = registerGuestDto.PhoneNumber,
            Address = registerGuestDto.Address,
            User = user
        };

        await _guestRepository.AddAsync(guest);
        await _unitOfWork.SaveChangesAsync();

        // Return both user and guest DTOs
        return (_mapper.Map<UserDto>(user), _mapper.Map<GuestDto>(guest));
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<string> GenerateJwtTokenAsync(UserDto user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
