using System;
using System.Collections.Generic;
using ToTypeScriptD.Lexical.TypeScript.Abstract;

namespace ToTypeScriptD.Lexical.TypeScript
{
    public class TSModule
    {
        public string Namespace { get; set; }
        public ICollection<TSModuleTypeDeclaration> TypeDeclarations { get; set; } = new List<TSModuleTypeDeclaration>();
        
        public override string ToString()
        {
            throw new NotImplementedException();

        }
    }
}
