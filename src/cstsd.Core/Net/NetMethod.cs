using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetMethod : NetMember
    {

        public bool IsConstructor { get; set; } = false;

        public NetType ReturnType { get; set; }

        public ICollection<NetParameter> Parameters { get; set; } = new List<NetParameter>();

        public string MethodBody { get; set; } = "/* function body */";

        public override string ToString()
        {
            return Name;
        }
    }
}
