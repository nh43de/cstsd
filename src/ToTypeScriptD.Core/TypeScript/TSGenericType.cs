using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSGenericType : TSType
    {
        public ICollection<TSType> GenericParameters { get; set; } = new List<TSType>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public TSGenericType(string name, string nameSpace) : base(name, nameSpace)
        {
        }
    }
}