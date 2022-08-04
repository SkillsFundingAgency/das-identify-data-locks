using System;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class EarningEventPriceEpisodeModel
    {
        public long Id { get; set; }
        public Guid EarningEventId { get; set; }
        public decimal TotalNegotiatedPrice1 { get; set; }
        public decimal TotalNegotiatedPrice2 { get; set; }
        public decimal TotalNegotiatedPrice3 { get; set; }
        public decimal TotalNegotiatedPrice4 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
    }
}