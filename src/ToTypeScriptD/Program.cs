using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical;

namespace cstsd
{
    class Program
    {

        //TODO: location of output file
        static void Main(string[] args)
        {
            TsdConfig config = null;
            IList<string> assemblyPaths = new string[] { };

            var options = new Options();

            string verbInvoked = null;

            string outputPath = null;
            var parseSuccess = false;

            if (Debugger.IsAttached)
            {
                parseSuccess = true;
                outputPath = "output.d.ts";

                assemblyPaths = new string[] {"cl.dll"};

                config = new TsdConfig
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
                    //verbInvoked = (verb ?? "").ToLowerInvariant();
                    outputPath = options.OutputFilePath;
                    assemblyPaths = options.Files;
                    
                    config = new TsdConfig
                    {
                        CamelBackCase = options.CamelBackCase,
                        IncludeSpecialTypes = options.IncludeSpecialTypeDefinitions,
                        IndentationType = options.IndentationType,
                        RegexFilter = options.RegexFilter,
                        RequireTypeScriptExportAttribute = !options.IncludeAllTypes
                    };
                });

            }


            if (!parseSuccess) return;
            bool skipPrintingHelp = true;
            try
            {
                if (string.IsNullOrWhiteSpace(outputPath))
                    Render.FromAssemblies(assemblyPaths, config, Console.Out);
                else
                {
                    Console.WriteLine($"Writing to output file: {outputPath}");

                    TextWriter w = new StreamWriter(outputPath, false);
                    
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
