using System;
using ToTypeScriptD.Core.TypeWriters;

namespace ToTypeScriptD.Core.WinMD
{
    public class WinMDTypeWriterTypeSelector : ITypeWriterTypeSelector
    {
        public ITypeWriter PickTypeWriter(Type td, int indentCount, ConfigBase config)
        {
            if (td.IsEnum)
            {
                return new EnumWriter(td, indentCount, config);
            }

            if (td.IsInterface)
            {
                return new InterfaceWriter(td, indentCount, config);
            }

            if (td.IsClass)
            {
                
                if (td.BaseType.FullName == "System.MulticastDelegate" ||
                    td.BaseType.FullName == "System.Delegate")
                {
                    return new DelegateWriter(td, indentCount, config);
                }

                return new ClassWriter(td, indentCount, config);
            }

            throw new NotImplementedException("Could not get a type to generate for:" + td.FullName);
        }
    }
}
