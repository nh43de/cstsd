using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using cstsd.Lexical.Core;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.Net;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class RenderTypescript
    {

        public static void FromAssemblies(ICollection<string> assemblyPaths, TsWriterConfig config, TextWriter w)
        {
            foreach (var aPath in assemblyPaths)
            {
                FromAssembly(aPath, config, w);
            }
        }

        public static void FromAssembly(string assemblyPath, TsWriterConfig config, TextWriter w)
        {
            throw new NotImplementedException();
        }

        public static void FromAssemblyControllerRoslyn(string textFilePath, string outputNamespace, TsWriterConfig config, TextWriter w)
        {
            var tc = new NetTsControllerConverter();

            var css = new RoslynTypeScanner();

            var netAssembly = css.RegisterCodeFile(outputNamespace, textFilePath);

            var ww = new TsWriter(config, w, netAssembly.Namespaces.Select(n => n.Name));

            w.Write(GetHeader(new[] { outputNamespace }));

            var netClasses = netAssembly.Namespaces.SelectMany(netNamespace => netNamespace.TypeDeclarations).OfType<NetClass>().ToList();

            var tsNamespace = new TsNamespace
            {
                Name = outputNamespace
            };

            foreach (var netClass in netClasses)
            {
                var module = tc.GetControllerTsModule(netClass);
                tsNamespace.Modules.Add(module);
            }
            
            w.Write(ww.WriteNamespace(tsNamespace, false));
        }


        public static void FromAssemblyController(string assemblyPath, TsWriterConfig config, TextWriter w)
        {
            var assembly = Assembly.LoadFile(new FileInfo(assemblyPath).FullName);

            var assemblyTypes = CsTypeScanner.GetAssemblyTypes(assembly);
            var controllerTypes = new List<Type>();
            foreach (var typeDeclaration in assemblyTypes)
            {
                if(typeDeclaration.Name.EndsWith("Controller"))
                {
                    controllerTypes.Add(typeDeclaration);
                }
            }

            var tc = new NetTsControllerConverter();

            var css = new CsTypeScanner();

            var netAssembly = css.RegisterAssembly(controllerTypes.ToArray(), assembly.FullName);

            var ww = new TsWriter(config, w, netAssembly.Namespaces.Select(n => n.Name));
            
            w.Write(GetHeader(new[] { assemblyPath }));

            var netClasses = netAssembly.Namespaces.SelectMany(netNamespace => netNamespace.TypeDeclarations).OfType<NetClass>().ToList();
            
            foreach (var netClass in netClasses)
            {
                //var bodyProperties = netClass.Properties.Where(p => p.IsPublic).Cast<NetType>();

                w.Write(ww.WriteModule(tc.GetControllerTsModule(netClass), true));
                w.Write(config.NewLines(2));
            }
            


        }




        public static void FromAssemblyPoco(string assemblyPath, TsWriterConfig config, TextWriter w, string namespaceOverride = "")
        {
            var assembly = Assembly.LoadFrom(new FileInfo(assemblyPath).FullName);

            var types = CsTypeScanner.GetAssemblyTypes(assembly);
            var pocoTypes = new List<Type>();
            foreach (var typeDeclaration in types)
            {
                var attributes = typeDeclaration.CustomAttributes.Select(a => a.AttributeType.Name).ToArray();

                if (attributes.Contains("TsExportAttribute"))
                {
                    pocoTypes.Add(typeDeclaration);
                }
            }
            
            var tc = new NetTsPocoConverter();

            var css = new CsTypeScanner();
            
            var netAssembly = css.RegisterAssembly(pocoTypes.ToArray(), string.IsNullOrWhiteSpace(namespaceOverride) ? assembly.FullName : namespaceOverride);
            
            var ww = new TsWriter(config, w, netAssembly.Namespaces.Select(n => n.Name));
            
            w.Write(GetHeader(new[] { assemblyPath }));
            
            var netClasses = netAssembly.Namespaces.SelectMany(netNamespace => netNamespace.TypeDeclarations).OfType<NetClass>().ToList();
            var netEnums = netAssembly.Namespaces.SelectMany(netNamespace => netNamespace.TypeDeclarations).OfType<NetEnum>().ToList();


            foreach (var netClass in netClasses)
            {
                if (true) //write interfaces {
                {                
                    w.Write(ww.WriteInterface(tc.GetTsInterface(netClass)));
                    w.Write(config.NewLines(2));
                }
                else //write interfaces in modules
                {
                    //var bodyProperties = netClass.Properties.Where(p => p.IsPublic).Cast<NetType>();
                    var tsModule = new TsModule
                    {
                        Name = netClass.Namespace,
                        TypeDeclarations = new[] { tc.GetTsInterface(netClass) }
                    };

                    w.Write(ww.WriteModule(tsModule, true));
                    w.Write(config.NewLines(2));
                }
               


            }
            
            foreach (var netEnum in netEnums)
            {
                //var bodyProperties = netClass.Properties.Where(p => p.IsPublic).Cast<NetType>();

                var tsModule = new TsModule
                {
                    Name = netEnum.Namespace,
                    TypeDeclarations = new[] {tc.GetTsEnum(netEnum)}
                };

                w.Write(ww.WriteModule(tsModule, true));
                w.Write(config.NewLines(2));
            }

            //File.WriteAllText("output2.d.ts", w.());
        }




        //TODO: typescriptexport attribute - flag to do all or look for attribute? what about namespace-level exports? inherited attribute? 
        //public static void FromAssembly(string assemblyPath, TsWriterConfig config, TextWriter w)
        //{
        //    var css = new CsTypeScanner();

        //    w.Write(GetHeader(new[] { assemblyPath }, config.IncludeSpecialTypes));

        //    var netAssembly = css.RegisterAssembly(assemblyPath);

        //    var ww = new TsWriter(config, w, netAssembly.Namespaces);

        //    var controllers = new List<NetClass>();
        //    var objects = new List<NetClass>();
        //    foreach (var netNamespace in netAssembly.Namespaces)
        //    {
        //        foreach (var typeDeclaration in netNamespace.TypeDeclarations)
        //        {
        //            var @class = typeDeclaration as NetClass;
        //            if (@class != null)
        //            {
        //                if (@class.Attributes.Contains("TypeScriptController"))
        //                {
        //                    controllers.Add(@class);
        //                }
        //                else if (@class.Attributes.Contains("TypeScriptObject"))
        //                {
        //                    objects.Add(@class);
        //                }
        //                else
        //                {
        //                    objects.Add(@class);
        //                }
        //            }
        //        }
        //    }

        //    foreach (var netClass in objects)
        //    {
        //        //var bodyProperties = netClass.Properties.Where(p => p.IsPublic).Cast<NetType>();

        //        w.Write(ww.WriteNamespace(netClass.Namespace, new[] {netClass}));
        //        w.Write(config.NewLines(2));
        //    }

        //    //File.WriteAllText("output2.d.ts", w.());
        //}




        private static string GetHeader(IEnumerable<string> assemblyPaths)
        {
            var sb = new StringBuilder();
            sb.AppendLine("//****************************************************************");
            sb.AppendLine("//  Generated by:  ToTypeScriptD");
            sb.AppendLine("//  Website:       http://github.com/ToTypeScriptD/ToTypeScriptD");
            sb.AppendLine($"//  Version:       {System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof (RenderTypescript).Assembly.Location).ProductVersion}");
            sb.AppendLine($"//  Date:          {DateTime.Now}");
            if (assemblyPaths.Any())
            {
                sb.AppendLine("//");
                sb.AppendLine("//  Assemblies:");
                assemblyPaths
                    .Select(System.IO.Path.GetFileName)
                    .Distinct()
                    .OrderBy(s => s)
                    .Each(path =>
                    {
                        sb.AppendLine($"//    {Path.GetFileName(path)}");
                    });
                sb.AppendLine("//");
            }
            sb.AppendLine("//****************************************************************");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }



    }
}
