using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public interface ITypeScanner<TType>
    {
        NetAssembly GetTsAssembly(ICollection<TType> types, string assemblyName);
    }
}
