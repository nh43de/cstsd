using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Lexical.TypeScript
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