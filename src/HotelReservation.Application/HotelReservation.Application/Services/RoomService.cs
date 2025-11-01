using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<RoomDto>> SearchRoomsAsync(string? searchTerm)
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _mapper.Map<IEnumerable<RoomDto>>(rooms);

            var filtered = rooms.Where(r =>
                (!string.IsNullOrEmpty(r.RoomNumber) && r.RoomNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (r.RoomType != null && !string.IsNullOrEmpty(r.RoomType.TypeName) && r.RoomType.TypeName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

            return _mapper.Map<IEnumerable<RoomDto>>(filtered);
        }

        public async Task<IEnumerable<RoomDto>> SearchAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            // Ensure dates are UTC to avoid Npgsql DateTime Kind errors when writing to timestamptz
            DateTime ToUtc(DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Utc) return dt;
                if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
                // Unspecified - treat as local and convert
                return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified).ToUniversalTime();
            }

            var checkInUtc = ToUtc(checkInDate);
            var checkOutUtc = ToUtc(checkOutDate);

            // Use repository method which includes RoomType and filters reserved rooms in DB
            var rooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync(checkInUtc, checkOutUtc);

            // filter by capacity (RoomType may be included by repository)
            var suitable = rooms.Where(r => r.RoomType != null && r.RoomType.Capacity >= numberOfGuests);

            return _mapper.Map<IEnumerable<RoomDto>>(suitable);
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int roomId)
        {
            var room = await _unitOfWork.Rooms.GetWithDetailsAsync(roomId);
            if (room == null) return null;
            return _mapper.Map<RoomDto>(room);
        }

        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync()
        {
            var types = await _unitOfWork.RoomTypes.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomTypeDto>>(types);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            // Ensure dates are UTC to avoid Npgsql DateTime Kind errors
            DateTime ToUtc(DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Utc) return dt;
                if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
                return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified).ToUniversalTime();
            }

            var checkInUtc = ToUtc(checkInDate);
            var checkOutUtc = ToUtc(checkOutDate);

            try
            {
                // prefer repository-level availability check if implemented
                return await _unitOfWork.Rooms.IsRoomAvailableAsync(roomId, checkInUtc, checkOutUtc);
            }
            catch
            {
                // If any repository/db error occurs, treat as not available to avoid 500s at API boundary.
                return false;
            }
        }
    }
}
