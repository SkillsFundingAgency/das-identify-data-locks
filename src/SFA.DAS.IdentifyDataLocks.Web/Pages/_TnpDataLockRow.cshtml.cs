using SFA.DAS.IdentifyDataLocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class AmountFromDate
    {
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }

    public class TnpDataLockRowModel
    {
        public string RowClass { get; }
        public string Heading { get; }
        public string? IlrValue { get; }

        public IEnumerable<AmountFromDate> IlrValues { get; set; } = new AmountFromDate[0];

        public TnpDataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, decimal?> valueExtractor)
        {
            RowClass = heading.ToLower().Replace(" ", "");
            Heading = heading;
            var ilr = period.Ilr;
            IlrValue = (ilr != null ? valueExtractor(ilr) : null)?.ToString("c") ?? "-";
            if (ilr != null)
            {
                IlrValues = ilr.Tnp1.Select(x => new AmountFromDate { Start = x.Item1, Amount = x.Item2 });
            }
        }

        public TnpDataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, List<(DateTime, decimal)>> valueExtractor)
        {
            RowClass = heading.ToLower().Replace(" ", "");
            Heading = heading;
            var ilr = period.Ilr;
            if (ilr != null)
            {
                IlrValues = valueExtractor(ilr).Select(x => new AmountFromDate { Start = x.Item1, Amount = x.Item2 });
                IlrValue = IlrValues.FirstOrDefault()?.Amount.ToString("c");
            }
        }
    }
}