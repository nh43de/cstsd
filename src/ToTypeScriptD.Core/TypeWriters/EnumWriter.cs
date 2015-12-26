using Mono.Cecil;
using System.Linq;
using System.Text;

namespace ToTypeScriptD.Core.TypeWriters
{
    public class EnumWriter : ITypeWriter
    {
        private readonly ConfigBase config;

        public EnumWriter(TypeDefinition typeDefinition, int indentCount, TypeCollection typeCollection, ConfigBase config)
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
            TypeDefinition.Fields.OrderBy(ob => ob.Constant).For((item, i, isLast) =>
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

        public TypeDefinition TypeDefinition { get; set; }

        public int IndentCount { get; set; }
    }
}
