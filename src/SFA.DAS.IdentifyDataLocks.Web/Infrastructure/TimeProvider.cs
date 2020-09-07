using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
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