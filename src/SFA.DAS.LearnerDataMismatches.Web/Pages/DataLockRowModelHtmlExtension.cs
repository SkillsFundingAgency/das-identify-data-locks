using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.LearnerDataMismatches.Domain;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public static class DataLockRowModelHtmlExtension
    {
        public static Task<IHtmlContent> PeriodDataLockRow(
            this IHtmlHelper htmlHelper,
            CollectionPeriod period,
            DataLock dlock,
            string heading,
            Func<DataMatch, object> value)
        {
            var model = new _DataLockRowModel(heading, dlock, period, value);
            return htmlHelper.PartialAsync("_DataLockRow", model);
        }

        public static Task<IHtmlContent> PeriodDataRow(
            this IHtmlHelper htmlHelper,
            CollectionPeriod period,
            string heading,
            Func<DataMatch, object> value)
        {
            var model = new _DataLockRowModel(heading, 0, period, value);
            return htmlHelper.PartialAsync("_DataLockRow", model);
        }
    }
}