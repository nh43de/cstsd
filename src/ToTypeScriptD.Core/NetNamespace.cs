using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetNamespace
    {
        public string Namespace { get; set; }
        public ICollection<NetType> TypeDeclarations { get; set; } = new List<NetType>();

        public override string ToString()
        {
            return Namespace;
        }
    }
}
