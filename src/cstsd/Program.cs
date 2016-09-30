using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using cstsd.TypeScript;
using Fclp;
using Fclp.Internals.Extensions;
using Newtonsoft.Json;

namespace cstsd
{
    class Program
    {
        //TODO: location of output file
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser();

            p.SetupHelp("?", "help")
                .Callback(helpText =>
                {
                    Console.WriteLine("Welcome to cstsd. Use the following command line args:");
                    Console.WriteLine(helpText);
                    Console.WriteLine("");
                    Console.WriteLine("");
                });
            
            var filePath = "cstsd.json";
            p.Setup<string>('c', "config")
                .Callback(s => filePath = s)
                .WithDescription("This is the path cstsd config json file or directory where cstsd.json is located.")
            ;

            var isDirFile = FileHelpers.IsDirFile(filePath);

            if (isDirFile == null)
            {
                Console.WriteLine($"Could not find '{Path.GetFullPath(filePath)}'...");
                return;
            }
            
            if (!isDirFile.Value)
                filePath = Path.Combine(filePath, "cstsd.json");

            TsWriterConfig cstsdConfig;
            if (File.Exists(filePath))
            {
                cstsdConfig = JsonConvert.DeserializeObject<TsWriterConfig>(File.ReadAllText(filePath));
            }
            else
            {
                Console.WriteLine($"Could not find '{filePath}'...");
                return;
            }

            var cstsdDir = new FileInfo(filePath).Directory?.FullName ?? "";


            //render controllers
            Console.WriteLine("Scanning controllers");
            if (cstsdConfig.ControllerTasks != null)
            {
                foreach (var controllerTask in cstsdConfig.ControllerTasks)
                {
                    Console.WriteLine($"Scanning controller: {controllerTask.SourceFile}");

                    var fileName = Path.GetFileNameWithoutExtension(controllerTask.SourceFile);

                    var outputFile = Path.IsPathRooted(controllerTask.OutputDirectory) == false
                        ? Path.Combine(cstsdDir, controllerTask.OutputDirectory, fileName + ".ts")
                        : Path.Combine(controllerTask.OutputDirectory, fileName + ".ts");

                    var nameSpace = string.IsNullOrWhiteSpace(controllerTask.Namespace)
                        ? cstsdConfig.DefaultControllerNamespace
                        : controllerTask.Namespace;

                    CheckCreateDir(outputFile);
                    using (TextWriter tw = new StreamWriter(outputFile, false))
                    {
                        RenderTypescript.FromControllerRoslyn(controllerTask.SourceFile, nameSpace, cstsdConfig,
                            tw);
                        tw.Flush();
                    }
                }
            }

            //render poco objects
            Console.WriteLine("Scanning poco objects");
            if (cstsdConfig.PocoObjectTasks != null)
            {
                //render poco's from one dll into one .d.ts file
                foreach (var pocoTask in cstsdConfig.PocoObjectTasks)
                {
                    var sourceFiles = new List<string>();
                    
                    pocoTask.SourceDirectories.ForEach(sd =>
                    {
                        Console.WriteLine($"Scanning poco dir: {sd}");
                        if(pocoTask.Recursive)
                            FileHelpers.ScanRecursive(sd, sourceFiles.Add);
                        else
                            FileHelpers.ScanStandard(sd, sourceFiles.Add);
                    });
                    
                    var outputFileName = pocoTask.OutputName; //make the outputfilename the na

                    var outputFile = Path.IsPathRooted(pocoTask.OutputDirectory) == false
                        ? Path.Combine(cstsdDir, pocoTask.OutputDirectory, outputFileName + ".d.ts")
                        : Path.Combine(pocoTask.OutputDirectory, outputFileName + ".d.ts");

                    var nameSpace = string.IsNullOrWhiteSpace(pocoTask.Namespace)
                        ? cstsdConfig.DefaultPocoNamespace
                        : pocoTask.Namespace;
                    
                    CheckCreateDir(outputFile);
                    using (TextWriter tw = new StreamWriter(outputFile, false))
                    {
                        RenderTypescript.FromPocoRoslyn(sourceFiles, nameSpace, cstsdConfig, tw);
                        tw.Flush();
                    }
                }
            }


            
            // Console.WriteLine(@"Press any key to continue...");
            // Console.ReadLine();
        }


        private static void CheckCreateDir(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);

            if (Directory.Exists(dirPath))
                return;

            var di = Directory.CreateDirectory(dirPath);
        }


    }
}
