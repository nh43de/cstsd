using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.Extensions
{

    public static class Extensions
    {
        public static string Dup(this string value, int count)
        {
            return string.Join("", Enumerable.Range(0, count).Select(s => value));
        }

        public static string ToTypeScript(this Type type)
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, string> _specialEnumNames = new Dictionary<string, string>
        {
            {"GB2312", "gb2312"},
            {"PC437", "pc437"},
            {"NKo", "nko"},
        };
        

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

        [System.Diagnostics.DebuggerHidden]
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

        [System.Diagnostics.DebuggerHidden]
        public static string Join(this IEnumerable<string> items, string separator = "")
        {
            if (items == null) return string.Empty;

            return string.Join(separator, items);
        }

        public static void NewLine(this System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("");
        }

        public static void AppendFormatLine(this System.Text.StringBuilder sb, string format=null, params object[] args)
        {
            if (!string.IsNullOrEmpty(format))
                sb.AppendFormat(format, args);
            sb.AppendLine();
        }
    }
}
