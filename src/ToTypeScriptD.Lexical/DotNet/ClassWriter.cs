using System;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD.Lexical.DotNet
{
    public class ClassWriter : TypeWriterBase
    {
        public ClassWriter(Type typeDefinition, int indentCount, DotNetConfig config)
            : base(typeDefinition, indentCount, config)
        {
        }

        public override void Write(System.Text.StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "interface", "extends");
        }
    }
}
