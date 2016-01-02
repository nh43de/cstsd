using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSClass : TSInterface
    {
        //TODO: replace \r\n to config 
        
        public ICollection<TSModuleTypeDeclaration> NestedClasses { get; set; } = new List<TSModuleTypeDeclaration>(); 

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}