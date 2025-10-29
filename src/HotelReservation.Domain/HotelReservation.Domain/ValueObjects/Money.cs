namespace HotelReservation.Domain.ValueObjects;

public record Money(decimal Amount)
{
    public static Money Zero => new Money(0m);
    
    public static Money operator +(Money left, Money right) => new Money(left.Amount + right.Amount);
    public static Money operator -(Money left, Money right) => new Money(left.Amount - right.Amount);
    public static Money operator *(Money money, decimal multiplier) => new Money(money.Amount * multiplier);
    
    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal amount) => new Money(amount);
}
