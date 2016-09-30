using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetMember
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; } = false;
        public ICollection<string> Attributes { get; set; } = new List<string>();
    }
}