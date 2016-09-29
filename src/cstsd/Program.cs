using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using cstsd.Lexical.TypeScript;
using Fclp;
using Newtonsoft.Json;

namespace cstsd
{
    public static class EnumerableExtensions
    {

        public static void For<T>(this IEnumerable<T> items, Action<T> itemAction)
        {
            foreach (var item in items)
            {
                itemAction(item);
            }
        }



    }

    public static class FileHelpers
    {
        /// <summary>
        /// Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool? IsDirFile(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path)) return null;
            var fileAttr = File.GetAttributes(path);
            return !fileAttr.HasFlag(FileAttributes.Directory);
        }
    }

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
            
            //render controllers
            foreach (var controllerTask in cstsdConfig.ControllerTasks)
            {
                var fileName = Path.GetFileNameWithoutExtension(controllerTask.SourceFile);
                var outputFile = Path.Combine(controllerTask.OutputDirectory, fileName + ".ts");
                var nameSpace = string.IsNullOrWhiteSpace(controllerTask.Namespace)
                    ? cstsdConfig.DefaultControllerNamespace
                    : controllerTask.Namespace;

                using (TextWriter tw = new StreamWriter(outputFile, false))
                {
                    RenderTypescript.FromAssemblyControllerRoslyn(controllerTask.SourceFile, nameSpace, cstsdConfig, tw);
                    tw.Flush();
                }
            }

            //render poco's from one dll into one .d.ts file
            foreach (var pocoTask in cstsdConfig.PocoObjectTasks)
            {
                var fileName = Path.GetFileNameWithoutExtension(pocoTask.SourceFile);
                var outputFile = Path.Combine(pocoTask.OutputDirectory, fileName + ".d.ts");
                var nameSpace = string.IsNullOrWhiteSpace(pocoTask.Namespace)
                    ? cstsdConfig.DefaultPocoNamespace
                    : pocoTask.Namespace;

                using (TextWriter tw = new StreamWriter(outputFile, false))
                {
                    RenderTypescript.FromAssemblyPoco(pocoTask.SourceFile, cstsdConfig, tw, nameSpace);
                    tw.Flush();
                }
            }
            
            // Console.WriteLine(@"Press any key to continue...");
            // Console.ReadLine();
        }

        public void Exit()
        {

            
            // Console.ReadLine();
        }

    }
}
