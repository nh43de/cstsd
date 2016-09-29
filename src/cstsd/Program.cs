using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using cstsd.Lexical.TypeScript;
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

    class Program
    {
        //TODO: location of output file
        static void Main(string[] args)
        {
            var filePath = "cstsd.json";
            var cstsdConfig = new TsWriterConfig();
            if (File.Exists(filePath))
            {
                cstsdConfig = JsonConvert.DeserializeObject<TsWriterConfig>(File.ReadAllText(filePath));
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
    }
}
