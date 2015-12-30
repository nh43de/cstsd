using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToTypeScriptD.Core.Attributes;
using ToTypeScriptD.Core.WinMD;

namespace ToTypeScriptD.Core.TypeWriters
{
    //TODO: use this instead of dictionary~string,itypewriter
    public class TsdTypeDefinition
    {
        public string Name { get; set; }
        public string NameSpace { get; set; }
        public ITypeWriter TypeWriter { get; set; }
    }

    public class TypeCollection
    {
        //TODO: rename to upper starting letter
        Dictionary<string, ITypeWriter> types = new Dictionary<string, ITypeWriter>();
        HashSet<string> typesRendered = new HashSet<string>();
        HashSet<Assembly> assemblies = new HashSet<Assembly>();

        public TypeCollection(ITypeWriterTypeSelector typeSelector)
        {
            TypeSelector = typeSelector;
        }

        public bool Contains(string name)
        {
            return types.ContainsKey(name);
        }

        public void Add(string @namespace, string name, ITypeWriter typeWriterBase)
        {
            if (name.ShouldIgnoreTypeByName())
                return;

            var fullname = @namespace + "." + name;


            // HACK:
            // Types in here are causing some issues - removing for now - will work on later
            if (fullname.StartsWith("Windows.UI.Input.Inking"))
            {
                return;
            }

            //if (name != nameof(TypeScriptExportAttribute))
            //    return;

            if (!types.ContainsKey(fullname))
            {
                types.Add(fullname, typeWriterBase);
            }
        }

        /// <summary>
        /// Renders a type collection beginning by namespace.
        /// </summary>
        /// <param name="filterRegex"></param>
        /// <returns></returns>
        public string Render(string filterRegex)
        {
            Func<string, string> getNamespace = name => name.Substring(0, name.LastIndexOf('.'));

            var items = from t in types
                        where !typesRendered.Contains(t.Key)
                        where t.Value.FullName.Matches(filterRegex)
                        orderby t.Key
                        group t by getNamespace(t.Key) into namespaces
                        select namespaces; //<- select namespaces 
            
            var sb = new StringBuilder();
            foreach (var ns in items)
            {
                //TS modules are namespaces
                sb.Append($@"declare module {ns.Key}");
                sb.AppendLine();
                sb.Append("{");
                sb.AppendLine();
                sb.AppendLine();

                foreach (var type in ns)
                {
                    typesRendered.Add(type.Key);
                    //type.Value is the TypeWriter instance to uses
                    type.Value.Write(sb);
                    sb.AppendLine();
                }

                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        internal void AddAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        internal Type LookupType(Type item)
        {
            string lookupName = item.FullName;
            if (item.IsConstructedGenericType)
            {
                lookupName = item.GetElementType().FullName;
            }
            var foundType = item.Module.GetTypes().SingleOrDefault(w => w.FullName == lookupName);
            return foundType;
        }

        public ITypeWriterTypeSelector TypeSelector { get; private set; }
    }

}
