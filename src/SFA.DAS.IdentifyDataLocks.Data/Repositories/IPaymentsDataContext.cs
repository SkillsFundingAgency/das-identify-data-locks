using Microsoft.EntityFrameworkCore;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Repositories;

public interface IPaymentsDataContext
{
    public DbSet<ApprenticeshipModel> Apprenticeship { get; set; }
    public DbSet<EarningEventModel> EarningEvent { get; set; }
    public DbSet<DataLockEventModel> DataLockEvent { get; set; }
}