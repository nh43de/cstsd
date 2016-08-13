using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetEnum : TSModuleTypeDeclaration
    {
        public HashSet<string> Enums { get; set; } = new HashSet<string>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
