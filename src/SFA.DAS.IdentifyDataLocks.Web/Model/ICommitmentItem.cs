namespace SFA.DAS.IdentifyDataLocks.Web.Model
{
    public interface ICommitmentItem
    {
        public string Type { get; }

        public decimal Amount { get; }
    }
}