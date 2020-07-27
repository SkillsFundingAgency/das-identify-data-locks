//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using PaymentTools.Model;
//using SFA.DAS.Payments.Application.Repositories;
//using SFA.DAS.Payments.Model.Core.Audit;
//using SFA.DAS.Payments.Model.Core.Entities;
//using System.Collections.Generic;
//using System.Linq;

//namespace LearnerDataMismatches.Pages
//{
//    public class PaymentsLearnerModel : PageModel
//    {
//        [BindProperty(SupportsGet = true)]
//        public string Uln { get; set; } = "9000000407";

//        public IEnumerable<CollectionPeriod> CollectionPeriods { get; private set; }

//        private readonly IPaymentsDataContext context;

//        public PaymentsLearnerModel(IPaymentsDataContext context) =>
//            this.context = context;

//        //public void OnGet()
//        //{
//        //    if (!long.TryParse(Uln, out var learnerUln))
//        //        throw new System.Exception("Invalid ULN");

//        //    var earnings = context.EarningEvent
//        //        .Where(x => x.LearnerUln == learnerUln)
//        //        .Include(x => x.PriceEpisodes)
//        //        .AsEnumerable()
//        //        .GroupBy(x => x.CollectionPeriod)
//        //        .SelectMany(x => x.OrderByDescending(y => y.IlrSubmissionDateTime).Take(1))
//        //        .GroupBy(x => x.CollectionPeriod)
//        //        .ToList();

//        //    var paidCommitments = context.Payment
//        //        .Where(x => x.LearnerUln == learnerUln)
//        //        //.Where(x => x.PriceEpisodeIdentifier)
//        //        .ToList()
//        //        ;

//        //    var lockedCommitmentsQueryable = context.DataLockgEvent
//        //        .Include(de => de.NonPayablePeriods)
//        //        .ThenInclude(npp => npp.DataLockEventNonPayablePeriodFailures)
//        //        .Where(x => x.LearnerUln == learnerUln);//.ToList();

//        //    var lockedCommitments = lockedCommitmentsQueryable.ToList();

//        //    var apprenticeships = context.Apprenticeship.Where(x => x.Uln == learnerUln).Include(x => x.ApprenticeshipPriceEpisodes).ToList();
//        //    var ape = apprenticeships.SelectMany(x => x.ApprenticeshipPriceEpisodes).ToList();

//        //    CollectionPeriods = earnings
//        //        .OrderBy(x => x.Key)
//        //        .Select(periodEarnings => new CollectionPeriod
//        //    {
//        //        PeriodName = $"R0{periodEarnings.Key}",
//        //        PriceEpisodes = periodEarnings
//        //                            .SelectMany(x => x.PriceEpisodes, (period, earning) => 
//        //                                MapPriceEpisode(period, earning, paidCommitments, lockedCommitments, apprenticeships)).ToList(),
//        //    }).ToList();
//        //}

//        private static PriceEpisode MapPriceEpisode(
//            EarningEventModel earning,
//            EarningEventPriceEpisodeModel episode,
//            List<PaymentModel> paidCommitments,
//            List<DataLockEventModel> lockedCommitments,
//            List<ApprenticeshipModel> commitments)
//        {
//            return new PriceEpisode(
//                episode.PriceEpisodeIdentifier,
//                earning.ContractType.ToString(),
//                episode.AgreedPrice,
//                commitments.SelectMany(x => x.ApprenticeshipPriceEpisodes, (c, x) => new Commitment
//                {
//                    Id = x.Id,
//                    Start = x.StartDate,
//                    End = x.EndDate,
//                    Employer = c.AccountId,
//                    Provider = earning.Ukprn,
//                    Course = FrameworkString(c),
//                    Status = c.Status,
//                    Cost = x.Cost,
//                    Payments = paidCommitments
//                        .Where(y => y.PriceEpisodeIdentifier == episode.PriceEpisodeIdentifier)
//                        .Where(y => y.CollectionPeriod.Period == earning.CollectionPeriod)
//                        .Select(y => new Payment
//                        {
//                            Id = y.Id,
//                            Amount = y.Amount,
//                            PriceEpisodeIdentifier = y.PriceEpisodeIdentifier,
//                            CollectionPeriod = y.CollectionPeriod,
//                            TransactionType = y.TransactionType.ToString(),
//                            DeliveryPeriod = y.DeliveryPeriod
//                        }).ToList(),
//                    DataLocked = lockedCommitments
//                        .Where(y => y.CollectionPeriod == earning.CollectionPeriod)
//                        .SelectMany(y => y.NonPayablePeriods)
//                        .SelectMany(y => y.DataLockEventNonPayablePeriodFailures)
//                        .Select(y => new DataLock
//                        {
//                            Amount = y.DataLockEventNonPayablePeriod.Amount,
//                            DataLockErrorCode = y.DataLockFailure,
//                            Id = y.Id,
//                            DeliveryPeriod = y.DataLockEventNonPayablePeriod.DeliveryPeriod
//                        }).ToList(),
//                    FrameworkCode = c.FrameworkCode,
//                    PathwayCode = c.PathwayCode,
//                    StandardCode = c.StandardCode,
//                    ProgrammeType = c.ProgrammeType,
//                    Ukprn = c.Ukprn,
//                    Uln = c.Uln
//                }),
//                earning.Ukprn,
//                earning.LearnerUln,
//                earning.LearningAimStandardCode,
//                earning.LearningAimFrameworkCode,
//                earning.LearningAimProgrammeType,
//                earning.LearningAimPathwayCode,
//                episode.StartDate);
//        }

//        private static string FrameworkString(ApprenticeshipModel c)
//        {
//            return $"{StandardString(c)}-{c.AgreedOnDate:dd-MM-yyyy}";

//        }

//        private static string StandardString(ApprenticeshipModel c)
//        {
//            return c.ProgrammeType == 0 ? $"25-{c.StandardCode}-" : $"{c.FrameworkCode}-{c.ProgrammeType}-{c.PathwayCode}";
//        }
//    }
//}
