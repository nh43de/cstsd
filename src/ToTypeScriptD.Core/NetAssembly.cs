using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetAssembly
    {
        public NetAssembly(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        
        public ICollection<NetModule> Modules { get; set; } = new List<NetModule>();
    }
}
