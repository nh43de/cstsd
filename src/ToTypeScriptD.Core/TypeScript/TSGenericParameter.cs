using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSGenericParameter : TSType
    {

        public ICollection<TSType> ParameterConstraints { get; set; } = new List<TSType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public TSGenericParameter(string name) : base(name, "")
        {
        }
    }
}