using System;
using ToTypeScriptD.Core.Config;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class ClassWriter : TypeWriterBase
    {
        public ClassWriter(Type typeDefinition, int indentCount, ConfigBase config)
            : base(typeDefinition, indentCount, config)
        {
        }

        public override void Write(System.Text.StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "class", "implements");
        }
    }
}
