using System.Collections.Generic;
using ToTypeScriptD.Core.TypeScript;

namespace ToTypeScriptD.Lexical
{
    public interface ITypeScanner<TType>
    {
        TSAssembly GetTSAssembly(ICollection<TType> types, string assemblyName);
    }
}