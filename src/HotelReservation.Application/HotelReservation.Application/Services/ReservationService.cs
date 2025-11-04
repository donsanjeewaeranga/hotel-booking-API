using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Services;

namespace HotelReservation.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IGuestRepository guestRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _guestRepository = guestRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto createReservationDto)
    {
        // Validate guest exists
        var guest = await _guestRepository.GetByIdAsync(createReservationDto.GuestId);
        if (guest == null)
            throw new KeyNotFoundException("Guest not found");

        // Validate room exists and is available - use GetWithDetailsAsync to load RoomType
        var room = await _roomRepository.GetWithDetailsAsync(createReservationDto.RoomId);
        if (room == null)
            throw new KeyNotFoundException("Room not found");

        if (!await _roomRepository.IsRoomAvailableAsync(createReservationDto.RoomId, createReservationDto.CheckInDate, createReservationDto.CheckOutDate))
            throw new InvalidOperationException("Room is not available for the selected dates");

        // Calculate totals
        var numberOfDays = (createReservationDto.CheckOutDate - createReservationDto.CheckInDate).Days;
        var (totalAmount, taxServiceCharge, grandTotal) = ReservationCalculationService.CalculateReservationTotals(
            room.RoomType.BasePrice, numberOfDays);

        // Create reservation
        var reservation = _mapper.Map<Domain.Entities.Reservation>(createReservationDto);
        reservation.TotalAmount = totalAmount.Amount;
        reservation.TaxServiceCharge = taxServiceCharge.Amount;
        reservation.GrandTotal = grandTotal.Amount;

        await _reservationRepository.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        // Return with details
        var createdReservation = await _reservationRepository.GetWithDetailsAsync(reservation.ReservationId);
        return _mapper.Map<ReservationDto>(createdReservation!);
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetWithDetailsAsync(reservationId);
        return reservation == null ? null : _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<IEnumerable<ReservationSummaryDto>> GetReservationsByGuestIdAsync(int guestId)
    {
        var reservations = await _reservationRepository.GetReservationsByGuestIdAsync(guestId);
        return _mapper.Map<IEnumerable<ReservationSummaryDto>>(reservations);
    }

    public async Task<bool> CancelReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            return false;

        try
        {
            reservation.Cancel();
            _reservationRepository.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> CheckInReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            return false;

        try
        {
            reservation.CheckIn();
            _reservationRepository.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> CheckOutReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            return false;

        try
        {
            reservation.CheckOut();
            _reservationRepository.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
