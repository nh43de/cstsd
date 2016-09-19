using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using cstsd.Lexical.Core;
using cstsd.Lexical.TypeScript;

namespace cstsd
{
    class Program
    {

        //TODO: location of output file
        static void Main(string[] args)
        {
            //RenderTypescript.FromAssemblies(assemblyPaths, config, Console.Out);

            var config2 = new TsWriterConfig
            {
                CamelBackCase = true,
                IndentationType = IndentationFormatting.SpaceX4
            };


            using (TextWriter tw = new StreamWriter(@"C:\code\test\eCovenantCloud.API.d.ts", false))
            {
                RenderTypescript.FromAssemblyController(@"C:\code\source\Git.eCovenantCloud.vNext\src\eCovenantCloud.API\bin\Debug\net461\eCovenantCloud.API.dll", config2, tw);

                tw.Flush();
            }

            using (TextWriter tw = new StreamWriter(@"C:\code\test\eCovenantCloud.DataAccess.d.ts", false))
            {
                RenderTypescript.FromAssemblyPoco(@"C:\code\test\eCovenantCloud.DataAccess.dll", config2, tw);

                tw.Flush();
            }

            using (TextWriter tw = new StreamWriter(@"C:\code\test\eCovenantCloud.Services.Core.d.ts", false))
            {
                RenderTypescript.FromAssemblyPoco(@"C:\code\test\eCovenantCloud.Services.Core.dll", config2, tw);

                tw.Flush();
            }

            Console.WriteLine(@"Press any key to continue...");
            Console.ReadLine();

            return;

            TsWriterConfig config = null;
            IList<string> assemblyPaths = new string[] { };

            var options = new Options();

            string verbInvoked = null;

            string outputPath = null;
            var parseSuccess = false;

            if (Debugger.IsAttached)
            {
                parseSuccess = true;
                outputPath = "output.d.ts";

                assemblyPaths = new string[] {"ecc.dll"};

                config = new TsWriterConfig
                {
                    CamelBackCase = true,
                    IndentationType = IndentationFormatting.SpaceX4
                };
            }
            else
            {
                parseSuccess = CommandLine.Parser.Default.ParseArgumentsStrict(args, options, () =>
                {
                    Console.WriteLine("Parsing failed");
                });

                Console.WriteLine("Done1");

                //verbInvoked = (verb ?? "").ToLowerInvariant();
                outputPath = options.OutputFilePath;
                assemblyPaths = options.Files;

                config = new TsWriterConfig
                {
                    CamelBackCase = options.CamelBackCase,
                    IndentationType = options.IndentationType,
                    RegexFilter = options.RegexFilter,
                    RequireTypeScriptExportAttribute = !options.IncludeAllTypes
                };
            }

            Console.WriteLine(parseSuccess);

            if (!parseSuccess) return;
            bool skipPrintingHelp = true;


            //try
            {
                if (string.IsNullOrWhiteSpace(outputPath))
                    RenderTypescript.FromAssemblies(assemblyPaths, config, Console.Out);
                else
                {
                    Console.WriteLine($"Writing to output file: {outputPath}");

                    TextWriter w = new StreamWriter(outputPath, false);

                    RenderTypescript.FromAssemblies(assemblyPaths, config, w);
                    
                    w.Flush();
                }
            }
            //catch (Exception ex)
            //{
            //    if (ex is System.IO.DirectoryNotFoundException || ex is System.IO.FileNotFoundException)
            //    {
            //        skipPrintingHelp = true;
            //        Console.Error.WriteLine("Error: " + ex.Message);
            //        Console.Read();
            //    }
            //    else
            //    {
            //        Console.Error.WriteLine("Error: " + ex.Message);

            //        Console.Read();

            //        throw;
            //    }
            //}

            Console.Read();

            if (skipPrintingHelp) return;
            Console.WriteLine(options.GetUsage(verbInvoked));
            Environment.ExitCode = 1;
        }
    }
}
