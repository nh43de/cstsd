using System.IO;
using System.Linq;
using cstsd.Core;
using cstsd.Core.Net;

namespace cstsd.TypeScript
{
    public static class RenderCs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textFilePath">Input .cs file</param>
        /// <param name="outputNamespace"></param>
        /// <param name="config"></param>
        /// <param name="w"></param>
        public static void FromControllerRoslyn(string textFilePath, string outputNamespace, WriterConfig config, TextWriter w)
        {
            // gather types using Roslyn: register files to the type scanner
            var css = new RoslynTypeScanner(outputNamespace);
            css.RegisterCodeFile(textFilePath);


            // write header to text stream
            w.Write(RenderingHelpers.GetHeader(new[] { outputNamespace }));
            
            // get classes
            var netClasses = css.NetAssembly.Namespaces.SelectMany(nn => nn.TypeDeclarations).OfType<NetClass>().ToList();
            
            //class that converts net types to CS proxies
            var tc = new NetCsControllerConverter();
            
            // create a new CS namespace for our proxy classes
            var netNamespace = new NetNamespace
            {
                Name = outputNamespace,
                ImportNamespaces = new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "RestSharp",
                    "PowerServices.Core"
                }
            };

            //add each class to the namespace
            foreach (var netClass in netClasses)
            {
                var a = tc.GetControllerApiClientCsClass(netClass);
                netNamespace.TypeDeclarations.Add(a);
            }

            // this will write our CS types
            var ww = new CsWriter(config, w, css.NetAssembly.Namespaces.Select(n => n.Name));

            w.Write(ww.WriteNamespace(netNamespace));
        }
    }
}