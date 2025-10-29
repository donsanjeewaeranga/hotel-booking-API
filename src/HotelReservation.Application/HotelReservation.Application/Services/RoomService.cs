using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IRoomTypeRepository roomTypeRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(RoomSearchDto searchDto)
    {
        var rooms = await _roomRepository.GetAvailableRoomsAsync(
            searchDto.CheckInDate, 
            searchDto.CheckOutDate, 
            searchDto.RoomTypeId);

        var roomDtos = new List<RoomDto>();
        foreach (var room in rooms)
        {
            var roomDto = _mapper.Map<RoomDto>(room);
            roomDtos.Add(roomDto);
        }

        return roomDtos;
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int roomId)
    {
        var room = await _roomRepository.GetWithDetailsAsync(roomId);
        return room == null ? null : _mapper.Map<RoomDto>(room);
    }

    public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync()
    {
        var roomTypes = await _roomTypeRepository.GetActiveRoomTypesAsync();
        return _mapper.Map<IEnumerable<RoomTypeDto>>(roomTypes);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        return await _roomRepository.IsRoomAvailableAsync(roomId, checkInDate, checkOutDate);
    }
}
