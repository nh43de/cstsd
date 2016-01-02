using System;
using System.Text;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class InterfaceWriter : TypeWriterBase
    {
        public InterfaceWriter(Type typeDefinition, int indentCount, ConfigBase config, ITypeWriterTypeSelector selector)
            : base(typeDefinition, indentCount, config, selector)
        {
        }

        public override void Write(StringBuilder sb)
        {
            ++IndentCount;
            //base.GetClass(sb, "interface", "extends");
        }
    }
}
