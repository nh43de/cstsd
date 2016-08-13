using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetGenericParameter
    {
        public string Name { get; set; }
        public Type ParameterType { get; set; }

        public ICollection<NetType> ParameterConstraints { get; set; } = new List<NetType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }

}