using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetGenericType : NetType
    {
        public ICollection<NetType> GenericParameters { get; set; } = new List<NetType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public NetGenericType(string name, string nameSpace) : base(name, nameSpace)
        {
        }
    }
}