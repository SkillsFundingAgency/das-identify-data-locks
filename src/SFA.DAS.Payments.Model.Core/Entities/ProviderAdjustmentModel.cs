﻿using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class ProviderAdjustmentModel
    {
        public long Ukprn { get; set; }
        public Guid SubmissionId { get; set; }
        public int SubmissionCollectionPeriod { get; set; }
        public int SubmissionAcademicYear { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        public decimal Amount { get; set; }
        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodYear { get; set; }
        public int CollectionPeriodMonth { get; set; }
    }
}


