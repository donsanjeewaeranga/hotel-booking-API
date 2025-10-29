using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Application.Services;

public class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepository;
    private readonly IMapper _mapper;

    public GuestService(IGuestRepository guestRepository, IMapper mapper)
    {
        _guestRepository = guestRepository;
        _mapper = mapper;
    }

    public async Task<GuestDto?> GetGuestByUserIdAsync(int userId)
    {
        var guest = await _guestRepository.GetByUserIdAsync(userId);
        return guest == null ? null : _mapper.Map<GuestDto>(guest);
    }

    public async Task<GuestDto> CreateGuestAsync(CreateGuestDto createGuestDto)
    {
        var guest = _mapper.Map<Domain.Entities.Guest>(createGuestDto);
        await _guestRepository.AddAsync(guest);
        return _mapper.Map<GuestDto>(guest);
    }

    public async Task<GuestDto> UpdateGuestAsync(int guestId, UpdateGuestDto updateGuestDto)
    {
        var guest = await _guestRepository.GetByIdAsync(guestId);
        if (guest == null)
            throw new KeyNotFoundException("Guest not found");

        _mapper.Map(updateGuestDto, guest);
        _guestRepository.Update(guest);
        return _mapper.Map<GuestDto>(guest);
    }

    public async Task<bool> DeleteGuestAsync(int guestId)
    {
        var guest = await _guestRepository.GetByIdAsync(guestId);
        if (guest == null)
            return false;

        guest.DeletedAt = DateTime.UtcNow;
        _guestRepository.Update(guest);
        return true;
    }
}
