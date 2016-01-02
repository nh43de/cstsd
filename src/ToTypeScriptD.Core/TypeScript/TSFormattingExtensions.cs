using System.Text.RegularExpressions;

namespace ToTypeScriptD.Core.TypeScript
{
    public static class TSFormattingExtensions
    {
        public static Regex IndentRegex = new Regex("^", RegexOptions.Compiled | RegexOptions.Multiline);

        public static string Indent(this string str, int spaces)
        {
            return IndentRegex.Replace(str, "".PadLeft(spaces));
        }

        public static string Indent(this string str, string indentionStr)
        {
            return IndentRegex.Replace(str, indentionStr);
        }

    }
}