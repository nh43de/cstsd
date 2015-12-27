using System;

namespace ToTypeScriptD.Core.TypeWriters
{
    public interface ITypeWriterTypeSelector
    {
        ITypeWriter PickTypeWriter(Type td, int indentCount, TypeCollection typeCollection, ConfigBase config);
    }
}
