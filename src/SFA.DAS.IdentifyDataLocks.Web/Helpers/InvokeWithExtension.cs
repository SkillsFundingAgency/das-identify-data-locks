using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{

    public static class InvokeWithExtension
    {
        public static R InvokeWith<T, R>(this T obj, Func<T, R> fn)
            where R : class
            =>
            obj != null ? fn?.Invoke(obj) ?? default : default;
    }
}