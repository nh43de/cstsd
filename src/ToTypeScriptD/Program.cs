using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical.DotNet;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD
{
    class Program
    {

        //TODO: location of output file
        static void Main(string[] args)
        {
            ConfigBase config = null;
            IList<string> assemblyPaths = new string[] { };

            var options = new Options();

            string verbInvoked = null;

            bool outputToFile = false;
            bool parseSuccess = false;

            if (Debugger.IsAttached)
            {
                parseSuccess = true;
                outputToFile = true;

                assemblyPaths = new string[] {"cl.dll"};

                config = new DotNetConfig
                {
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
                            assemblyPaths = dotNetSubOptions.Files;

                            config = new DotNetConfig
                            {
                                
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

                            assemblyPaths = winmdSubOptions.Files;

                            config = new WinmdConfig
                            {
                                IncludeSpecialTypes = winmdSubOptions.IncludeSpecialTypeDefinitions,
                                IndentationType = winmdSubOptions.IndentationType,
                                RegexFilter = winmdSubOptions.RegexFilter
                            };
                            break;
                    }
                });

            }


            if (!parseSuccess) return;
            bool skipPrintingHelp = true;
            try
            {
                if (!outputToFile)
                    Render.FromAssemblies(assemblyPaths, config, Console.Out);
                else
                {
                    Console.WriteLine(@"Writing to output file: output.d.ts");

                    TextWriter w = new StreamWriter(@"output.d.ts", false);
                    
                    Render.FromAssemblies(assemblyPaths, config, w);
                    
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
