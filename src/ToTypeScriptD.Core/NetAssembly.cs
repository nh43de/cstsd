using System.Collections.Generic;
using System.Reflection;

namespace ToTypeScriptD.Core
{
    public class NetAssembly
    {
        public string Name { get; set; }

        
        public ICollection<NetNamespace> Namespaces { get; set; } = new List<NetNamespace>();
    }
}
