using System;
using System.Collections.Generic;
using ToTypeScriptD.Lexical.TypeScript.Abstract;

namespace ToTypeScriptD.Lexical.TypeScript
{
    public class TSClass : TSInterface
    {
        public ICollection<TSModuleTypeDeclaration> NestedClasses { get; set; } = new List<TSModuleTypeDeclaration>(); 

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}