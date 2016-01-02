using System;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class WinMDTypeWriterTypeSelector : ITypeWriterTypeSelector
    {
        public ITypeWriter PickTypeWriter(Type td, int indentCount, ConfigBase config)
        {
            if (td.IsEnum)
            {
                return new EnumWriter(td, indentCount, config, this);
            }

            if (td.IsInterface)
            {
                return new InterfaceWriter(td, indentCount, config, this);
            }

            if (td.IsClass)
            {
                if (td.BaseType.FullName == "System.MulticastDelegate" ||
                    td.BaseType.FullName == "System.Delegate")
                {
                    return new DelegateWriter(td, indentCount, config, this);
                }

                return null; //new ClassWriter(td, indentCount, config, this);
            }

            throw new NotImplementedException("Could not get a type to generate for:" + td.FullName);
        }
    }
}
