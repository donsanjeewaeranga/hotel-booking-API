using HotelReservation.Domain.Enums;

namespace HotelReservation.Application.DTOs;

public class RoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public RoomTypeDto RoomType { get; set; } = null!;
    public RoomStatus Status { get; set; }
    public List<RoomFeatureDto> Features { get; set; } = new();
}

public class RoomTypeDto
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
}

public class RoomFeatureDto
{
    public int RoomFeatureId { get; set; }
    public string FeatureName { get; set; } = string.Empty;
}

public class RoomSearchDto
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int? RoomTypeId { get; set; }
    public int? MinCapacity { get; set; }
    public decimal? MaxPrice { get; set; }
}
