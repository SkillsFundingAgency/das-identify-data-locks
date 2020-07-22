using System.Collections.Generic;
using System.Linq;

namespace PaymentTools.Model
{
    public class CollectionPeriod
    {
        public string PeriodName { get; set; } = "R11";
        public decimal TotalPayments => PriceEpisodes.SelectMany(x => x.Commitments).Sum(x => x.TotalPayments);
        public decimal TotalLocked => PriceEpisodes.SelectMany(x => x.Commitments).SelectMany(x => x.Items).Select(x => x as DataLock).Where(x => x != null).Sum(x => x.Amount);
        public List<PriceEpisode> PriceEpisodes { get; set; }

        public List<DataLock> DataLocks => PriceEpisodes
            .SelectMany(x => x.Commitments)
            .SelectMany(x => x.DataLocked)
            .ToList();

        public List<string> UniqueDataLockNames =>
            DataLocks
            .Select(l => l.DataLockErrorCode.ToString())
            .Distinct()
            .ToList();

    }
}