using System.Collections.Generic;
using ToTypeScriptD.Core.Net;

namespace ToTypeScriptD.Core
{
    public interface ITypeScanner<TType>
    {
        NetAssembly GetNetAssembly(ICollection<TType> types, string assemblyName);
    }
}
