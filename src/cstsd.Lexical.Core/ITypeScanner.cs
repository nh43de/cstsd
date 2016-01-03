using System.Collections.Generic;
using ToTypeScriptD.Lexical.TypeScript;

namespace ToTypeScriptD.Lexical
{
    public interface ITypeScanner<TType>
    {
        TSAssembly GetTSAssembly(ICollection<TType> types, string assemblyName);
    }
}