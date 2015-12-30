using System;
using ToTypeScriptD.Core.Config;

namespace ToTypeScriptD.Lexical.TypeWriters
{
    public interface ITypeWriterTypeSelector
    {
        ITypeWriter PickTypeWriter(Type td, int indentCount, ConfigBase config);
    }
}
