using System;
using System.Text;
using ToTypeScriptD.Core.Config;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class InterfaceWriter : TypeWriterBase
    {
        public InterfaceWriter(Type typeDefinition, int indentCount, ConfigBase config)
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
