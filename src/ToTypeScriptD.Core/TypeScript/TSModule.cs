using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
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
