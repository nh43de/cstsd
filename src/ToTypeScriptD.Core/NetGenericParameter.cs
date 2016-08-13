using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetGenericParameter : NetType
    {

        public ICollection<NetType> ParameterConstraints { get; set; } = new List<NetType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public NetGenericParameter(string name) : base(name, "")
        {
        }
    }
}