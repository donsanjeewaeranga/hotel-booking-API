using HotelReservation.Domain.Enums;

namespace HotelReservation.Application.DTOs;

public class ReservationDto
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxServiceCharge { get; set; }
    public decimal GrandTotal { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public RoomDto Room { get; set; } = null!;
    public GuestDto Guest { get; set; } = null!;
}

public class CreateReservationDto
{
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

public class ReservationSummaryDto
{
    public int ReservationId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal GrandTotal { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public int NumberOfDays { get; set; }
}
