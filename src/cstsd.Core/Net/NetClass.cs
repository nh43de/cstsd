using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetClass : NetInterface
    {
        public ICollection<NetType> NestedClasses { get; set; } = new List<NetType>();

        public NetMethod Constructor { get; set; }
    }
}