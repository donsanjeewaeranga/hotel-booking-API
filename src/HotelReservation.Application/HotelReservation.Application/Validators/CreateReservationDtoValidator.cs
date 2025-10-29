using FluentValidation;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Validators;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.GuestId)
            .GreaterThan(0)
            .WithMessage("Guest ID must be greater than 0");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Room ID must be greater than 0");

        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Check-in date is required")
            .GreaterThan(DateTime.Today)
            .WithMessage("Check-in date must be in the future");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty()
            .WithMessage("Check-out date is required")
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date");

        RuleFor(x => x.CheckOutDate)
            .Must((dto, checkOutDate) => (checkOutDate - dto.CheckInDate).Days <= 30)
            .WithMessage("Reservation cannot exceed 30 days");
    }
}
