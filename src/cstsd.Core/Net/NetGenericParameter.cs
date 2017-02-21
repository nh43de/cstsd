using System.Collections.Generic;
using System.Linq;

namespace cstsd.Core.Net
{
    public class NetGenericParameter
    {
        public string Name { get; set; }
        
        public ICollection<NetGenericParameter> NetGenericParameters { get; set; } = new List<NetGenericParameter>();
        
        public ICollection<NetType> ParameterConstraints { get; set; } = new List<NetType>();

        public NetGenericParameter()
        {

        }

        public NetGenericParameter(NetType netType)
        {
            this.Name = netType.Name;

            this.NetGenericParameters = netType.GenericParameters;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}