using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetClass : NetInterface
    {
        public ICollection<TSModuleTypeDeclaration> NestedClasses { get; set; } = new List<TSModuleTypeDeclaration>(); 

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}