using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.IdentifyDataLocks.Domain;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class TnpDataLockRowModel
    {
        public string RowClass { get; }
        public string Heading { get; }
        public IEnumerable<AmountFromDate> IlrValues { get; set; }

        public TnpDataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, List<AmountFromDate>> valueExtractor)
        {
            RowClass = heading.ToLower().Replace(" ", "");
            Heading = heading;

            IlrValues = (period.IlrEarningDataMatch != null)
                ? valueExtractor(period.IlrEarningDataMatch)
                : Enumerable.Empty<AmountFromDate>();
        }
    }
}