using System;

namespace SFA.DAS.IdentifyDataLocks.Domain;

public class AmountFromDate
{
    public AmountFromDate(DateTime start, decimal amount)
    {
        (Start, Amount) = (start, amount);
    }

    public static implicit operator AmountFromDate((DateTime start, decimal amount) x)
    {
        return new AmountFromDate(x.start, x.amount);
    }

    public DateTime Start { get; set; }
    public decimal Amount { get; set; }

    public override string ToString()
    {
        return $"{Amount:c} from {Start.ToShortDateString()}";
    }
}