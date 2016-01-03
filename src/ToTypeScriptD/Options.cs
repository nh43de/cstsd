using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using ToTypeScriptD.Core;

namespace cstsd
{
    public class Options
    {
        public const string DotNetCommandName = "dotnet";
        public const string WinmdCommandName = "winmd";
        
        [ValueList(typeof(List<string>))]
        public IList<string> Files { get; set; }

        
        [Option('a', "includeAllTypes",
            HelpText = "Writes all assembly types regardless of whether TypeScriptExport attributes are present")]
        public bool IncludeAllTypes { get; set; } = false;


        [Option('s', "specialTypes", HelpText = "Writes the ToTypeScriptD special types to standard out")]
        public bool IncludeSpecialTypeDefinitions { get; set; }

        [Option('i', "indentWith", 
            HelpText = "Override default indentation of SpaceX4 (four spaces). Possible options: [None, TabX1, TabX2, SpaceX1,...SpaceX8]")]
        public IndentationFormatting IndentationType { get; set; } = IndentationFormatting.SpaceX4;

        [Option('o', "Output to File", HelpText = "Output results to file.")]
        public string OutputFilePath { get; set; } = null;

        //TODO: not implemented
        private string _regexFilter;
        [Option('r', "regexFilter", HelpText = "A .net regular expression that can be used to filter the FullName of types exported. Picture this taking the FullName of the TypeScript type and running it through the .Net Regex.IsMatch(name, pattern)")]
        public string RegexFilter
        {
            get
            {
                return _regexFilter;
            }
            set
            {
                var v = value ?? "";
                if (v.StartsWith("'") && v.EndsWith("'"))
                {
                    v = v.Substring(1, v.Length - 2);
                }
                if (v.StartsWith("\"") && v.EndsWith("\""))
                {
                    v = v.Substring(1, v.Length - 2);
                }

                _regexFilter = v;
            }
        }

        [Option('c', "camelBack", HelpText = "CamelBack case (lower case first letter)")]
        public bool CamelBackCase { get; set; }
        
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            var help = new HelpText
            {
                Heading = HeadingInfo.Default,
                Copyright = CopyrightInfo.Default,
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            //object optionsObject = null;
            //if (verb == DotNetCommandName)
            //{
            //    help.AddPreOptionsLine(Environment.NewLine + "Usage: ToTypeScriptD dotnet [--specialTypes] [File1.dll]...[FileN.dll]");
            //    optionsObject = new DotNetSubOptions();
            //}
            //else if (verb == WinmdCommandName)
            //{
            //    help.AddPreOptionsLine(Environment.NewLine + "Usage: ToTypeScriptD winmd [--specialTypes] [File1.winmd]...[FileN.winmd]");
            //    optionsObject = new WinmdSubOptions();
            //}

            help.AddDashesToOption = true;
            help.AddOptions(this);

            return help;
        }
    }
    
}
