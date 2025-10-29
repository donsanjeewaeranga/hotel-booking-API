namespace HotelReservation.Domain.ValueObjects;

public record DateRange(DateTime Start, DateTime End)
{
    public int Days => (End - Start).Days;
    
    public bool IsValid => End > Start;
    
    public bool Overlaps(DateRange other)
    {
        return Start < other.End && End > other.Start;
    }
}
