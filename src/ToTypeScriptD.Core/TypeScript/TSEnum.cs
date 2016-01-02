using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
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
