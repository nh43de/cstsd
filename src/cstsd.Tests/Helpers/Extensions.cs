namespace cstsd.Tests.Helpers
{
    public static class Extensions
    {
        public static string StripVersionFromOutput(this string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, "v[0-9].[0-9].[0-9]{0,4}.[0-9]{0,4}[0-9]? - SHA1:[a-zA-Z0-9]{0,7} - (Debug|Release)", "v0.0.0000.0000 SHA1:0000000 - Debug");
        }

        public static string StripHeaderGarbageromOutput(this string value)
        {
            value = System.Text.RegularExpressions.Regex.Replace(value, "__ToTypeScriptD_([0-9a-z]){32}:", "__ToTypeScriptD_{RANDOM_GUIDishString}:");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"//  Date:          (.*)(PM|AM)", "//  Date:          mm/dd/YYYY H:MM:SS PM");
            value = value.StripVersionFromOutput();
            return value;
        }
    }
}
