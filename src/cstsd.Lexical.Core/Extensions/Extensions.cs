using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using ToTypeScriptD.Core;

namespace ToTypeScriptD.Lexical.Extensions
{

    public static class Extensions
    {


        /// <summary>
        /// For iterator extension method that also includes a bool with the 'isLastItem' value.
        /// </summary>
        /// <example>
        ///     new[] { 1, 2, 3 }.For((item, i, isLastItem) => { });
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Items to iterate</param>
        /// <param name="action">generic action with T1=Item, T2=i</param>
        [System.Diagnostics.DebuggerHidden]
        public static void For<T>(this IEnumerable<T> items, Action<T, int, bool> action)
        {
            if (items != null)
            {
                var count = items.Count();
                int i = 0;
                foreach (var item in items)
                {
                    action(item, i, i == (count - 1));
                    i++;
                }
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static IEnumerable<int> Times(this int value)
        {
            return Enumerable.Range(0, value);
        }



        [DebuggerHidden]
        public static string FormatWith(this string format, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, format, args);
        }

        [DebuggerHidden]
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    action(item);
                }
            }
        }

        public static void AppendFormatLine(this StringBuilder sb, string format = null, params object[] args)
        {
            if (!string.IsNullOrEmpty(format))
                sb.AppendFormat(format, args);
            sb.AppendLine();
        }


    }
}
