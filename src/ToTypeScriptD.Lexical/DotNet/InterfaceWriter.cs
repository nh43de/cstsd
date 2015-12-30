using System;
using System.Text;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD.Lexical.DotNet
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
