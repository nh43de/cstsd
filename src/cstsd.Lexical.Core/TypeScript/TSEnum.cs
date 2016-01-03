using System;
using System.Collections.Generic;
using ToTypeScriptD.Lexical.TypeScript.Abstract;

namespace ToTypeScriptD.Lexical.TypeScript
{
    public class TSEnum : TSModuleTypeDeclaration
    {
        public HashSet<string> Enums { get; set; } = new HashSet<string>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
