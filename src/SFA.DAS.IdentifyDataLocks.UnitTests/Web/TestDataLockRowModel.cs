using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web
{
    public class TestDataLockRowModel
    {
        [Test, AutoData]
        public void Extracts_values_from_correct_datamatch(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch.Cost.Should().NotBe(period.IlrEarningDataMatch.Cost);

            var sut = new DataLockRowModel(period, "", m => m.Cost);

            sut.ApprenticeValue.Should().Be(period.ApprenticeshipDataMatch.Cost.ToString());
            sut.IlrValue.Should().Be(period.IlrEarningDataMatch.Cost.ToString());
        }

        [Test, AutoData]
        public void Copes_with_null_apprenticeship(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch = null;

            var sut = new DataLockRowModel(period, "", m => m.Cost);

            sut.ApprenticeValue.Should().Be("-");
        }

        [Test, AutoData]
        public void Copes_with_null_datamatch_value(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch.Standard = null;
            period.IlrEarningDataMatch.Standard = null;

            var sut = new DataLockRowModel(period, "", m => m.Standard);

            sut.ApprenticeValue.Should().Be("-");
            sut.IlrValue.Should().Be("-");
        }

        [Test, AutoData]
        public void Copes_with_null_extractor(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch.Standard = null;
            period.IlrEarningDataMatch.Standard = null;

            var sut = new DataLockRowModel(period, "", _ => null);

            sut.ApprenticeValue.Should().Be("-");
            sut.IlrValue.Should().Be("-");
        }

        [Test, AutoData]
        public void Reports_datalock_name(CollectionPeriod period)
        {
            var datalock = period.DataLockErrorCodes[0];

            var sut = new DataLockRowModel(period, datalock, "", _ => null);

            sut.IsLocked.Should().BeTrue();
            sut.ActiveDataLock.Should().Be(datalock.ToString());
        }

        [Test, AutoData]
        public void Reports_dash_for_absent_datalock(CollectionPeriod period)
        {
            period.DataLockErrorCodes.Clear();

            var sut = new DataLockRowModel(period, DataLockErrorCode.Dlock01, "", _ => null);

            sut.IsLocked.Should().BeFalse();
            sut.ActiveDataLock.Should().Be("-");
        }

        [Test, AutoData]
        public void Reports_pause_datalock_name(CollectionPeriod period)
        {
            var datalock = period.DataLockErrorCodes[0];

            var sut = new PauseDateDataLockRowModel(period, datalock, "");

            sut.IsLocked.Should().BeTrue();
            sut.ActiveDataLock.Should().Be(datalock.ToString());
        }

        [Test, AutoData]
        public void Reports_dash_for_absent_pause_datalock([Frozen] CollectionPeriod period, PauseDateDataLockRowModel sut)
        {
            period.DataLockErrorCodes.Clear();

            sut.IsLocked.Should().BeFalse();
            sut.ActiveDataLock.Should().Be("-");
        }

        [Test, AutoData]
        public void Reports_pause_start_date([Frozen] CollectionPeriod period, PauseDateDataLockRowModel sut)
        {
            sut.HasPausedDate.Should().BeTrue();
            sut.PausedOnDate.Should().Be(period.ApprenticeshipDataMatch.PausedOn!.Value.ToShortDateString());
        }

        [Test, AutoData]
        public void Reports_not_paused(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch.PausedOn = null;

            var sut = new PauseDateDataLockRowModel(period, DataLockErrorCode.Dlock12, "");

            sut.HasPausedDate.Should().BeFalse();
            sut.PausedOnDate.Should().BeNull();
        }

        [Test, AutoData]
        public void Reports_resumed([Frozen] CollectionPeriod period, PauseDateDataLockRowModel sut)
        {
            sut.ResumedOnDate.Should().Be(period.ApprenticeshipDataMatch.ResumedOn!.Value.ToShortDateString());
        }

        [Test, AutoData]
        public void Reports_not_resumed(CollectionPeriod period)
        {
            period.ApprenticeshipDataMatch.ResumedOn = null;

            var sut = new PauseDateDataLockRowModel(period, DataLockErrorCode.Dlock12, "");

            sut.ResumedOnDate.Should().Be("Present");
        }
    }
}