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

        public static string ToTypeScript(this Type type, TsdConfig config = null)
        {
            if (config == null)
                config = new TsdConfig();

            throw new NotImplementedException();
        }

        // TODO: look to move this to the WinMDExtensions.cs
        public static string ToTypeScriptItemNameWinMD(this Type typeReference)
        {
            // Nested classes don't report their namespace. So we have to walk up the 
            // DeclaringType tree to find the root most type to grab it's namespace.
            var parentMostType = typeReference;
            while (parentMostType.DeclaringType != null)
            {
                parentMostType = parentMostType.DeclaringType;
            }

            var mainTypeName = typeReference.FullName;

            // trim namespace off of the front.
            mainTypeName = mainTypeName.Substring(parentMostType.Namespace.Length + 1);

            // replace the nested class slash with an underscore
            mainTypeName = mainTypeName.Replace("/", "_").Replace("+", "_").StripGenericTick();

            mainTypeName = mainTypeName.StripGenericTick();
            return mainTypeName;
        }
        
        static Dictionary<string, string> _specialEnumNames = new Dictionary<string, string>
        {
            {"GB2312", "gb2312"},
            {"PC437", "pc437"},
            {"NKo", "nko"},
        };

        // Copied and modified from Json.Net
        // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/StringUtils.cs
        public static string ToCamelCase(this string s, bool camelCaseConfig)
        {
            if (!camelCaseConfig)
                return s;

            if (_specialEnumNames.ContainsKey(s))
            {
                return _specialEnumNames[s];
            }

            if (String.IsNullOrEmpty(s))
                return s;

            if (!Char.IsUpper(s[0]))
                return s;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                bool hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || Char.IsUpper(s[i + 1]))
                {
                    char lowerCase;
#if !(NETFX_CORE || PORTABLE)
                    lowerCase = Char.ToLower(s[i], CultureInfo.InvariantCulture);
#else
                    lowerCase = char.ToLower(s[i]);
#endif

                    sb.Append(lowerCase);
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

        public static string StripGenericTick(this string value)
        {
            4.Times().Each(x =>
            {
                value = value.Replace("`" + x, "");
            });
            return value;
        }
    }
}
