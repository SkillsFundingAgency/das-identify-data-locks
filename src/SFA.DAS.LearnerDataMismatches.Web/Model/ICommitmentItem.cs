namespace SFA.DAS.LearnerDataMismatches.Web.Model
{
    public interface ICommitmentItem
    {
        public string Type { get; }

        public decimal Amount { get; }
    }
}