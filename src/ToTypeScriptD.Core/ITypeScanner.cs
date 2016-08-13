using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public interface ITypeScanner<TType>
    {
        NetAssembly GetNetAssembly(ICollection<TType> types, string assemblyName);
    }
}
