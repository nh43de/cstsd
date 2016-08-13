using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetType
    {
        public string Namespace { get; set; }
        public string Name { get; set; }

        public ICollection<NetType> GenericParameters { get; set; } = new List<NetType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}