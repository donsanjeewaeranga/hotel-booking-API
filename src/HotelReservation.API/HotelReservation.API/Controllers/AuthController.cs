using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.Validators;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var validator = new CreateUserDtoValidator();
            var validationResult = await validator.ValidateAsync(createUserDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _authService.RegisterAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", createUserDto.Email);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email: {Email}", createUserDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred during registration." });
        }
    }

    [HttpPost("register/guest")]
    public async Task<ActionResult<object>> RegisterGuest([FromBody] RegisterGuestDto registerGuestDto)
    {
        try
        {
            var validator = new RegisterGuestDtoValidator();
            var validationResult = await validator.ValidateAsync(registerGuestDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var (user, guest) = await _authService.RegisterGuestAsync(registerGuestDto);
            var token = await _authService.GenerateJwtTokenAsync(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
            {
                user,
                guest,
                token
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Guest registration failed for email: {Email}", registerGuestDto.Email);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during guest registration for email: {Email}", registerGuestDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred during registration." });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var validator = new LoginDtoValidator();
            var validationResult = await validator.ValidateAsync(loginDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _authService.LoginAsync(loginDto);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            
            return Ok(new
            {
                user,
                token,
                expiresIn = 3600 // 1 hour
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", loginDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred during login." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        // This would typically require authentication and authorization
        // For now, returning a placeholder
        return NotFound();
    }
}
