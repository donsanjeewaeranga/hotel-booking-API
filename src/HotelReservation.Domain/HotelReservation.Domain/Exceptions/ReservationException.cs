namespace HotelReservation.Domain.Exceptions;

public class ReservationException : DomainException
{
    public ReservationException(string message) : base(message)
    {
    }
    
    public ReservationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
