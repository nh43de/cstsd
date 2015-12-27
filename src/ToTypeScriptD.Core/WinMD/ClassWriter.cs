using System;
using ToTypeScriptD.Core.TypeWriters;

namespace ToTypeScriptD.Core.WinMD
{
    public class ClassWriter : TypeWriterBase
    {
        public ClassWriter(Type typeDefinition, int indentCount, TypeCollection typeCollection, ConfigBase config)
            : base(typeDefinition, indentCount, typeCollection, config)
        {
        }

        public override void Write(System.Text.StringBuilder sb)
        {
            ++IndentCount;
            base.WriteOutMethodSignatures(sb, "class", "implements");
        }
    }
}
