using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.LearnerDataMismatches.Web.Infrastructure;
using SFA.DAS.LearnerDataMismatches.Web.Model;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public class LearnerModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uln { get; set; }

        public IEnumerable<Model.CollectionPeriod> CollectionPeriods { get; private set; }
        public IEnumerable<Domain.CollectionPeriod> NewCollectionPeriods { get; private set; }

        public string LearnerName { get; set; }

        public IEnumerable<string> DataLockNames =>
            CollectionPeriods
                .SelectMany(c => c.UniqueDataLockNames)
                .Distinct()
                .OrderBy(x => x);

        public string EmployerName { get; set; }
        public string EmployerId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }

        private readonly IPaymentsDataContext context;
        private readonly ICommitmentsService commitmentsService;
        private readonly IEmployerService employerService;
        private readonly IProviderService providerService;

        public LearnerModel(IPaymentsDataContext context, ICommitmentsService commitmentsService, IEmployerService employerService, IProviderService providerService)
        {
            this.context = context;
            this.commitmentsService = commitmentsService;
            this.employerService = employerService;
            this.providerService = providerService;
        }

        public async Task OnGetAsync()
        {
            if (!long.TryParse(Uln, out var learnerUln))
                throw new Exception("Invalid ULN");

            LearnerName = "Unknown Learner Name";

            await BuildCollectionPeriods(learnerUln);
            await BuildNewCollections(learnerUln);
            var learnerTask = Task.CompletedTask;// QueryLearner(learnerUln);

            await Task.WhenAll(learnerTask);
        }

        private async Task BuildNewCollections(long learnerUln)
        {
            var apprenticeships = await context.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Uln == learnerUln)
                .ToListAsync();

            var earnings = await context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == learnerUln)
                .ToListAsync();

            var report = new LearnerReport(apprenticeships, earnings);

            NewCollectionPeriods = report.CollectionPeriods;

            NewCollectionPeriods = NewCollectionPeriods
                .GroupBy(x => x.Period)
                .Select(x => x.First())
                .OrderByDescending(x => x);

            var activeAppreticeship = apprenticeships.FirstOrDefault(a =>
                a.Status == Payments.Model.Core.Entities.ApprenticeshipStatus.Active);

            if (activeAppreticeship != null)
            {
                await PopulateAppreticesDetails(activeAppreticeship.AccountId);
                await PopulateEmployerDetails(activeAppreticeship.AccountId);
                PopulateProviderDetails(activeAppreticeship.Ukprn);
            }
        }

        private void PopulateProviderDetails(long ukprn)
        {
            ProviderName = providerService.GetProviderName(ukprn);
            ProviderId = ukprn.ToString();
        }

        private async Task PopulateEmployerDetails(long accountId)
        {
            EmployerName = await employerService.GetEmployerName(accountId);
        }

        private async Task PopulateAppreticesDetails(long accountId)
        {
            LearnerName = await commitmentsService.GetApprenticesName(Uln, accountId);
        }

        //private Task QueryLearner(long learnerUln)
        //{
        //    commitments.GetApprenticeshipUpdates
        //}

        private async Task BuildCollectionPeriods(long learnerUln)
        {
            var a = await context.EarningEvent
                .Where(x => x.LearnerUln == learnerUln)
                .ToListAsync();

            var earnings = await GetEarningEvents(learnerUln);

            var paidCommitments = await GetCommitments(learnerUln);

            var lockedCommitments = await GetLockedCommitments(learnerUln);

            var apprenticeships = await GetApprenticeships(learnerUln);

            CollectionPeriods = earnings.OrderBy(x => x.Key).Select(periods => new Model.CollectionPeriod
            {
                PeriodName = $"R0{periods.Key}",
                PriceEpisodes = periods.SelectMany(x => x.PriceEpisodes, (period, earning) => MapPriceEpisode(period, earning, paidCommitments, lockedCommitments, apprenticeships)).ToList(),
            }).ToList();
        }

        private async Task<List<IGrouping<byte, EarningEventModel>>> GetEarningEvents(long learnerUln)
        {
            //var a = Bob(learnerUln);
            var bs = await GetEarningEventsBase(learnerUln);
            var cs = bs
                .GroupBy(x => x.CollectionPeriod)
                .ToList();

            return cs
                .SelectMany(x => x/*.OrderByDescending(y => y.IlrSubmissionDateTime)*/.Take(1))
                .GroupBy(x => x.CollectionPeriod)
                .ToList()
                ;
        }

        private async Task<List<EarningEventModel>> GetEarningEventsBase(long learnerUln)
        {
            return await context.EarningEvent
                .Where(x => x.LearnerUln == learnerUln)
                .Include(x => x.PriceEpisodes)
                .ToListAsync();
        }

        private object Bob(long learnerUln)
        {
            return context.EarningEvent
                .Where(x => x.LearnerUln == learnerUln).ToListAsync();
        }

        private async Task<List<PaymentModel>> GetCommitments(long learnerUln)
        {
            var paidCommitments = await context.Payment
                .Where(x => x.LearnerUln == learnerUln)
                //.Where(x => x.PriceEpisodeIdentifier)
                .ToListAsync()
                ;
            return paidCommitments;
        }

        private async Task<List<DataLockEventModel>> GetLockedCommitments(long learnerUln)
        {
            var lockedCommitmentsQueryable = context.DataLockgEvent
                .Include(de => de.NonPayablePeriods)
                .ThenInclude(npp => npp.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == learnerUln);//.ToList();

            var lockedCommitments = await lockedCommitmentsQueryable.ToListAsync();
            return lockedCommitments;
        }

        private async Task<List<ApprenticeshipModel>> GetApprenticeships(long learnerUln)
        {
            var apprenticeships = await context.Apprenticeship.Where(x => x.Uln == learnerUln).Include(x => x.ApprenticeshipPriceEpisodes).ToListAsync();
            var ape = apprenticeships.SelectMany(x => x.ApprenticeshipPriceEpisodes).ToList();
            return apprenticeships;
        }

        private static PriceEpisode MapPriceEpisode(
            EarningEventModel earning,
            EarningEventPriceEpisodeModel episode,
            List<PaymentModel> paidCommitments,
            List<DataLockEventModel> lockedCommitments,
            List<ApprenticeshipModel> commitments)
        {
            return new PriceEpisode(
                episode.PriceEpisodeIdentifier,
                earning.ContractType.ToString(),
                episode.AgreedPrice,
                commitments.SelectMany(x => x.ApprenticeshipPriceEpisodes, (c, x) => new Commitment
                {
                    Id = x.Id,
                    Start = x.StartDate,
                    End = x.EndDate,
                    Employer = c.AccountId,
                    Provider = earning.Ukprn,
                    Course = FrameworkString(c),
                    Status = c.Status,
                    Cost = x.Cost,
                    Payments = paidCommitments
                        .Where(y => y.PriceEpisodeIdentifier == episode.PriceEpisodeIdentifier)
                        .Where(y => y.CollectionPeriod.Period == earning.CollectionPeriod)
                        .Select(y => new Payment
                        {
                            Id = y.Id,
                            Amount = y.Amount,
                            PriceEpisodeIdentifier = y.PriceEpisodeIdentifier,
                            CollectionPeriod = y.CollectionPeriod,
                            TransactionType = y.TransactionType.ToString(),
                            DeliveryPeriod = y.DeliveryPeriod
                        }).ToList(),
                    DataLocked = lockedCommitments
                        .Where(y => y.CollectionPeriod == earning.CollectionPeriod)
                        .SelectMany(y => y.NonPayablePeriods)
                        .SelectMany(y => y.DataLockEventNonPayablePeriodFailures)
                        .Select(y => new Model.DataLock
                        {
                            Amount = y.DataLockEventNonPayablePeriod.Amount,
                            DataLockErrorCode = y.DataLockFailure,
                            Id = y.Id,
                            DeliveryPeriod = y.DataLockEventNonPayablePeriod.DeliveryPeriod
                        }).ToList(),
                    FrameworkCode = c.FrameworkCode,
                    PathwayCode = c.PathwayCode,
                    StandardCode = c.StandardCode,
                    ProgrammeType = c.ProgrammeType,
                    Ukprn = c.Ukprn,
                    Uln = c.Uln
                }),
                earning.Ukprn,
                earning.LearnerUln,
                earning.LearningAimStandardCode,
                earning.LearningAimFrameworkCode,
                earning.LearningAimProgrammeType,
                earning.LearningAimPathwayCode,
                episode.StartDate);
        }

        private static string FrameworkString(ApprenticeshipModel c)
        {
            return $"{StandardString(c)}-{c.AgreedOnDate:dd-MM-yyyy}";
        }

        private static string StandardString(ApprenticeshipModel c)
        {
            return c.ProgrammeType == 0 ? $"25-{c.StandardCode}-" : $"{c.FrameworkCode}-{c.ProgrammeType}-{c.PathwayCode}";
        }
    }
}