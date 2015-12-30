using System;
using System.Linq;
using System.Text;
using ToTypeScriptD.Core.DotNet;

namespace ToTypeScriptD.Core.TypeWriters
{
    public class EnumWriter : ITypeWriter
    {
        private readonly ConfigBase config;

        public EnumWriter(Type typeDefinition, int indentCount, ConfigBase config)
        {
            this.config = config;
            TypeDefinition = typeDefinition;
            IndentCount = indentCount;
        }

        public void Write(StringBuilder sb)
        {
            ++IndentCount;
            sb.AppendLine(IndentValue + "enum " + TypeDefinition.ToTypeScriptItemName() + " {");
            ++IndentCount;
            TypeDefinition.GetFields().OrderBy(ob => ob.Name).For((item, i, isLast) => //.OrderBy(ob => ob.Name)
            {
                if (item.Name == "value__") return;
                sb.AppendFormat("{0}{1}", IndentValue, item.Name.ToCamelCase(config.CamelBackCase));
                sb.AppendLine(isLast ? "" : ",");
            });
            --IndentCount;
            sb.AppendLine(IndentValue + "}");
        }

        public string IndentValue => config.Indent.Dup(IndentCount);

        public string FullName => TypeDefinition.Namespace + "." + TypeDefinition.ToTypeScriptItemName();

        public Type TypeDefinition { get; set; }

        public int IndentCount { get; set; }
    }
}
