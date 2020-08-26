using System.Collections.Generic;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public class DataLockHelpCentreLink
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public static DataLockHelpCentreLink Create(Domain.DataLock @lock)
        {
            links.TryGetValue(@lock, out var link);

            return new DataLockHelpCentreLink
            {
                Name = @lock.ToString(),
                Description = link.description,
                Url = MistmatchArticleUrl,
            };
        }

        private const string MistmatchArticleUrl = "https://help.apprenticeships.education.gov.uk/hc/en-gb/articles/360008448299-Data-mismatch-errors";

        private static readonly Dictionary<Domain.DataLock, (string url, string description)> links
            = new Dictionary<Domain.DataLock, (string url, string description)>
            {
                {Domain.DataLock.Dlock01, ("", "No matching UKPRN record found") },
                {Domain.DataLock.Dlock02, ("", "No matching ULN found.") },
                {Domain.DataLock.Dlock03, ("", "No matching standard code found.") },
                {Domain.DataLock.Dlock04, ("", "No matching framework code found.") },
                {Domain.DataLock.Dlock05, ("", "No matching programme type code found.") },
                {Domain.DataLock.Dlock06, ("", "No matching pathway code found.") },
                {Domain.DataLock.Dlock07, ("", "No matching negotiated cost of training and assessment found.") },
                {Domain.DataLock.Dlock08, ("", "Multiple matching records found.") },
                {Domain.DataLock.Dlock09, ("", "The start date for this negotiated price is before the corresponding price start date in the employer digital account") },
                {Domain.DataLock.Dlock10, ("", "The employer has stopped payments for this apprentice") },
                {Domain.DataLock.Dlock11, ("", "The employer is not currently a levy payer") },
                {Domain.DataLock.Dlock12, ("", "The employer has paused payments for this apprentice") },
            };

    }
}