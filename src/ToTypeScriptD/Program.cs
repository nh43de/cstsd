using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ToTypeScriptD;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Config;

namespace tsd
{
    class Program
    {

        //TODO: location of output file
        static void Main(string[] args)
        {
            ConfigBase config = null;

            var options = new Options();

            string verbInvoked = null;

            bool outputToFile = false;
            bool parseSuccess = false;

            if (Debugger.IsAttached)
            {
                parseSuccess = true;
                outputToFile = true;

                config = new ToTypeScriptD.Core.DotNet.DotNetConfig
                {
                    AssemblyPaths = new[] {"cl.dll"},
                    CamelBackCase = true,
                    IncludeSpecialTypes = true,
                    IndentationType = IndentationFormatting.SpaceX4
                };
            }
            else
            {
             
                parseSuccess = CommandLine.Parser.Default.ParseArgumentsStrict(args, options, (verb, subOptions) =>
                {
                    verbInvoked = (verb ?? "").ToLowerInvariant();
                    switch (verbInvoked)
                    {
                        case Options.DotNetCommandName:
                            var dotNetSubOptions = subOptions as DotNetSubOptions;
                            if (dotNetSubOptions == null) break;

                            outputToFile = dotNetSubOptions.OutputToFile;

                            config = new ToTypeScriptD.Core.DotNet.DotNetConfig
                            {
                                AssemblyPaths = dotNetSubOptions.Files,
                                CamelBackCase = dotNetSubOptions.CamelBackCase,
                                IncludeSpecialTypes = dotNetSubOptions.IncludeSpecialTypeDefinitions,
                                IndentationType = dotNetSubOptions.IndentationType,
                                RegexFilter = dotNetSubOptions.RegexFilter
                            };                        
                            break;
                        case Options.WinmdCommandName:
                            var winmdSubOptions = subOptions as WinmdSubOptions;
                            if (winmdSubOptions == null) break;

                            outputToFile = winmdSubOptions.OutputToFile;

                            config = new ToTypeScriptD.Core.WinMD.WinmdConfig
                            {
                                AssemblyPaths = winmdSubOptions.Files,
                                IncludeSpecialTypes = winmdSubOptions.IncludeSpecialTypeDefinitions,
                                IndentationType = winmdSubOptions.IndentationType,
                                RegexFilter = winmdSubOptions.RegexFilter
                            };
                            break;
                    }
                });

            }


            if (!parseSuccess) return;
            bool skipPrintingHelp;
            try
            {
                if (!outputToFile)
                    skipPrintingHelp = Render.AllAssemblies(config, Console.Out);
                else
                {
                    Console.WriteLine(@"Writing to output file: output.d.ts");

                    TextWriter w = new StreamWriter(@"output.d.ts", false);
                    
                    skipPrintingHelp = Render.AllAssemblies(config, w);
                    
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                if (ex is System.IO.DirectoryNotFoundException || ex is System.IO.FileNotFoundException)
                {
                    skipPrintingHelp = true;
                    Console.Error.WriteLine("Error: " + ex.Message);
                }
                else
                {
                    throw;
                }
            }

            if (skipPrintingHelp) return;
            Console.WriteLine(options.GetUsage(verbInvoked));
            Environment.ExitCode = 1;
        }
    }
}
