using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetNamespace
    {
        public string Name { get; set; }
        public IList<NetType> TypeDeclarations { get; set; } = new List<NetType>();

        public ICollection<string> ImportNamespaces { get; set; } = new List<string>();

        public override string ToString()
        {
            return Name;
        }
    }
}
