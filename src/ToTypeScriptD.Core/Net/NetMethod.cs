using System.Collections.Generic;

namespace ToTypeScriptD.Core.Net
{
    public class NetMethod : NetMember
    {

        public bool IsConstructor { get; set; } = false;

        public NetType ReturnType { get; set; }

        public ICollection<NetParameter> Parameters { get; set; } = new List<NetParameter>();

        public override string ToString()
        {
            return Name;
        }
    }
}
