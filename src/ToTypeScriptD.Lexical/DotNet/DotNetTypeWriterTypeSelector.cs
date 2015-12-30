using System;
using ToTypeScriptD.Core.TypeWriters;

namespace ToTypeScriptD.Core.DotNet
{
    public class DotNetTypeWriterTypeSelector : ITypeWriterTypeSelector
    {
        public ITypeWriter PickTypeWriter(Type td, int indentCount, ConfigBase config)
         {
            var castedConfig = (DotNetConfig)config;
            if (td.IsEnum)
            {
                return new EnumWriter(td, indentCount, config);
            }

            if (td.IsInterface)
            {
                return new InterfaceWriter(td, indentCount, castedConfig);
            }

            if (td.IsClass)
            {
                return new ClassWriter(td, indentCount, castedConfig);
            }

            throw new NotImplementedException("Could not get a type to generate for:" + td.FullName);
        }
    }
}
