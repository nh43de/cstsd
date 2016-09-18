using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class TsType
    {
        public string Name { get; set; }

        public bool IsPublic { get; set; } = true;
        
        public ICollection<TsGenericParameter> GenericParameters { get; set; } = new List<TsGenericParameter>();

        public override string ToString()
        {
            return Name;
        }
    }


}