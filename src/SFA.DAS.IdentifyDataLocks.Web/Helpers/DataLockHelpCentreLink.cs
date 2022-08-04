using System.Collections.Generic;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Web.Helpers
{
    public class DataLockHelpCentreLink
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public static DataLockHelpCentreLink Create(DataLockErrorCode dataLock)
        {
            Links.TryGetValue(dataLock, out var description);

            return new DataLockHelpCentreLink
            {
                Name = dataLock.ToString(),
                Description = description,
                Url = MistmatchArticleUrl,
            };
        }

        private const string MistmatchArticleUrl = "https://help.apprenticeships.education.gov.uk/hc/en-gb/articles/360008448299-Data-mismatch-errors";

        private static readonly Dictionary<DataLockErrorCode, string> Links = new Dictionary<DataLockErrorCode, string>
        {
            { DataLockErrorCode.Dlock01, "No matching UKPRN record found" },
            { DataLockErrorCode.Dlock02, "No matching ULN found." },
            { DataLockErrorCode.Dlock03, "No matching standard code found." },
            { DataLockErrorCode.Dlock04, "No matching framework code found." },
            { DataLockErrorCode.Dlock05, "No matching programme type code found." },
            { DataLockErrorCode.Dlock06, "No matching pathway code found." },
            { DataLockErrorCode.Dlock07, "No matching negotiated cost of training and assessment found." },
            { DataLockErrorCode.Dlock08, "Multiple matching records found." },
            { DataLockErrorCode.Dlock09, "The start date for this negotiated price is before the corresponding price start date in the employer digital account" },
            { DataLockErrorCode.Dlock10, "The employer has stopped payments for this apprentice" },
            { DataLockErrorCode.Dlock11, "The employer is not currently a levy payer" },
            { DataLockErrorCode.Dlock12, "The employer has paused payments for this apprentice" },
        };
    }
}