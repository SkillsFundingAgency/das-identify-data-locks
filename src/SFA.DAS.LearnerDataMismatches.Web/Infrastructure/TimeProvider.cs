using System;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public interface ITimeProvider
    {
        DateTime Today { get; }
    }

    public class TimeProvider : ITimeProvider
    {
        public DateTime Today => DateTime.Today;
    }
}