using System;

namespace SFA.DAS.IdentifyDataLocks.Domain.Services
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