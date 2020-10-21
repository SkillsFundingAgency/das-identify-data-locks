using SFA.DAS.IdentifyDataLocks.Domain;
using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class TnpDataLockRowModel
    {
        public string Heading { get; }
        public string? IlrValue { get; }

        public TnpDataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, decimal?> valueExtractor)
        {
            Heading = heading;
            var ilr = period.Ilr;
            IlrValue = (ilr != null ? valueExtractor(ilr) : null)?.ToString("c") ?? "-";
        }
    }
}