using System;
using System.Text;
using ToTypeScriptD.Core.TypeWriters;

namespace ToTypeScriptD.Core.DotNet
{
    public class InterfaceWriter : TypeWriterBase
    {
        public InterfaceWriter(Type typeDefinition, int indentCount, DotNetConfig config)
            : base(typeDefinition, indentCount, config)
        {
        }

        public override void Write(StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "interface", "extends");
        }
    }
}
