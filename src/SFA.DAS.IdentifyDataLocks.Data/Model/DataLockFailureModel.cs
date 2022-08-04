using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Data.Model;

public class DataLockFailureModel
{
    public long Ukprn { get; set; }
    public byte CollectionPeriod { get; set; }
    public short AcademicYear { get; set; }
    public List<DataLockErrorCode> DataLockFailures { get; set; }
}