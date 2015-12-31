using System;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.DotNet
{
    public class DotNetTypeWriterTypeSelector : ITypeWriterTypeSelector
    {
        public ITypeWriter PickTypeWriter(Type td, int indentCount, ConfigBase config)
        {
            var typeName = td.Name;
            var castedConfig = (DotNetConfig)config;
            if (td.IsEnum)
            {
                return new EnumWriter(td, indentCount, config, this);
            }

            if (td.IsInterface)
            {
                return new InterfaceWriter(td, indentCount, castedConfig, this);
            }

            if (td.IsClass)
            {
                return new ClassWriter(td, indentCount, castedConfig, this);
            }

            throw new NotImplementedException("Could not get a type to generate for:" + td.FullName);
        }
    }
}
