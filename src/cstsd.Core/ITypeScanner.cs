using System.Collections.Generic;
using cstsd.Core.Net;

namespace cstsd.Core
{
    public interface ITypeScanner<TType>
    {
        NetAssembly GetNetAssembly(ICollection<TType> types, string assemblyName);
    }
}
