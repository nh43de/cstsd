using System.Collections.Generic;

namespace ToTypeScriptD.Core.Net
{
    public class NetAssembly
    {
        public string Name { get; set; }

        
        public ICollection<NetNamespace> Namespaces { get; set; } = new List<NetNamespace>();

        public override string ToString()
        {
            return Name;
        }
    }
}
