using System;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class ClassWriter : TypeWriterBase
    {
        public ClassWriter(Type typeDefinition, int indentCount, ConfigBase config, ITypeWriterTypeSelector selector)
            : base(typeDefinition, indentCount, config, selector)
        {
        }

        public override void Write(System.Text.StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "class", "implements");
        }
    }
}
