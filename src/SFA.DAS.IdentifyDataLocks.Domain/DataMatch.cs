using System;
using System.Collections.Generic;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class DataMatch
    {
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public long? Standard { get; set; }
        public int? Framework { get; set; }
        public int? Program { get; set; }
        public int? Pathway { get; set; }
        public DateTime? PriceStart { get; set; }
        public DateTime? PausedOn { get; set; }
        public DateTime? ResumedOn { get; set; }
        public DateTime? StoppedOn { get; set; }
        public DateTime? PlannedCompletion { get; set; }
        public ApprenticeshipStatus CompletionStatus { get; set; }
        public DateTime IlrSubmissionDate { get; set; }
        public decimal Cost{ get; set; }

        public List<AmountFromDate> Tnp1 { get; set; }
        public List<AmountFromDate> Tnp2 { get; set; }
        public List<AmountFromDate> Tnp3 { get; set; }
        public List<AmountFromDate> Tnp4 { get; set; }
    }
}