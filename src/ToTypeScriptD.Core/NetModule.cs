using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetModule
    {
        public string Namespace { get; set; }
        public ICollection<TSModuleTypeDeclaration> TypeDeclarations { get; set; } = new List<TSModuleTypeDeclaration>();
        
        public override string ToString()
        {
            throw new NotImplementedException();

        }
    }
}
