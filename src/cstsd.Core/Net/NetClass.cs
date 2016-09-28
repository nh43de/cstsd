using System.Collections.Generic;

namespace ToTypeScriptD.Core.Net
{
    public class NetClass : NetInterface
    {
        public ICollection<NetType> NestedClasses { get; set; } = new List<NetType>(); 

    }
}