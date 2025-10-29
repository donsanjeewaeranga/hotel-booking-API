using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateReservationAsync(CreateReservationDto createReservationDto);
    Task<ReservationDto?> GetReservationByIdAsync(int reservationId);
    Task<IEnumerable<ReservationSummaryDto>> GetReservationsByGuestIdAsync(int guestId);
    Task<bool> CancelReservationAsync(int reservationId);
    Task<bool> CheckInReservationAsync(int reservationId);
    Task<bool> CheckOutReservationAsync(int reservationId);
}
