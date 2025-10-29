using HotelReservation.Domain.Enums;
using HotelReservation.Domain.ValueObjects;

namespace HotelReservation.Domain.Entities;

public class Reservation
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxServiceCharge { get; set; }
    public decimal GrandTotal { get; set; }
    public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Confirmed;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public Guest Guest { get; set; } = null!;
    public Room Room { get; set; } = null!;
    
    // Domain methods
    public DateRange GetDateRange() => new(CheckInDate, CheckOutDate);
    
    public int GetNumberOfDays() => GetDateRange().Days;
    
    public bool CanBeCanceled() => ReservationStatus == ReservationStatus.Confirmed;
    
    public void Cancel()
    {
        if (!CanBeCanceled())
            throw new InvalidOperationException("Reservation cannot be canceled in its current status.");
        
        ReservationStatus = ReservationStatus.Canceled;
    }
    
    public void CheckIn()
    {
        if (ReservationStatus != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed reservations can be checked in.");
        
        ReservationStatus = ReservationStatus.CheckedIn;
    }
    
    public void CheckOut()
    {
        if (ReservationStatus != ReservationStatus.CheckedIn)
            throw new InvalidOperationException("Only checked-in reservations can be checked out.");
        
        ReservationStatus = ReservationStatus.CheckedOut;
    }
}
