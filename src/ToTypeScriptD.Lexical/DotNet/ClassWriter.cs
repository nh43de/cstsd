using System;
using ToTypeScriptD.Lexical.TypeWriters;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD.Lexical.DotNet
{
    public class ClassWriter : TypeWriterBase
    {
        public ClassWriter(Type typeDefinition, int indentCount, DotNetConfig config, ITypeWriterTypeSelector selector)
            : base(typeDefinition, indentCount, config, selector)
        {
        }

        public override void Write(System.Text.StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "interface", "extends");
        }
    }
}
