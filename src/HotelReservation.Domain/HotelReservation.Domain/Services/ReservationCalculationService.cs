using HotelReservation.Domain.ValueObjects;

namespace HotelReservation.Domain.Services;

public static class ReservationCalculationService
{
    private const decimal TaxRate = 0.09m;
    
    public static Money CalculateTotalAmount(decimal basePrice, int numberOfDays)
    {
        return new Money(basePrice * numberOfDays);
    }
    
    public static Money CalculateTaxServiceCharge(Money totalAmount)
    {
        return new Money(totalAmount.Amount * TaxRate);
    }
    
    public static Money CalculateGrandTotal(Money totalAmount, Money taxServiceCharge)
    {
        return totalAmount + taxServiceCharge;
    }
    
    public static (Money totalAmount, Money taxServiceCharge, Money grandTotal) CalculateReservationTotals(
        decimal basePrice, int numberOfDays)
    {
        var totalAmount = CalculateTotalAmount(basePrice, numberOfDays);
        var taxServiceCharge = CalculateTaxServiceCharge(totalAmount);
        var grandTotal = CalculateGrandTotal(totalAmount, taxServiceCharge);
        
        return (totalAmount, taxServiceCharge, grandTotal);
    }
}
