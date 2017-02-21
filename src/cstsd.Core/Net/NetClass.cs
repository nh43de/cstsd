using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetClass : NetInterface
    {
        public ICollection<NetType> NestedClasses { get; set; } = new List<NetType>();
        public new ICollection<NetFieldDeclaration> Fields { get; set; } = new List<NetFieldDeclaration>();

        public NetMethod Constructor { get; set; }
    }
}