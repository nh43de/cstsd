using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToTypeScriptD.Core.Extensions;

namespace cstsd.Lexical.TypeScript.Extensions
{
    public static class LexicalExtensions
    {
        static Dictionary<string, string> _specialEnumNames = new Dictionary<string, string>
        {
            {"GB2312", "gb2312"},
            {"PC437", "pc437"},
            {"NKo", "nko"},
        };

        public static bool ShouldIgnoreTypeByName(this string name)
        {
            if (name == "<Module>")
                return true;

            if (name.StartsWith("__I") && name.EndsWith("PublicNonVirtuals"))
                return true;

            if (name.StartsWith("__I") && name.EndsWith("ProtectedNonVirtuals"))
                return true;

            return false;
        }
        
        // Copied and modified from Json.Net
        // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/StringUtils.cs
        public static string ToCamelCase(this string s)
        {
            if (_specialEnumNames.ContainsKey(s))
            {
                return _specialEnumNames[s];
            }

            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || char.IsUpper(s[i + 1]))
                {
                    var lowerCase = char.ToLower(s[i]);

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

        public static string ToTypeScriptCase(this string name)
        {
            if (name.ToUpper() == name)
            {
                return name.ToLower();
            }

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        public static string StripOutParamSymbol(this string value)
        {
            return value.Replace("&", "");
        }

        public static void NewLine(this System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("");
        }
    }
}
