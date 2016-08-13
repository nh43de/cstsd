using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetType
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsPublic { get; set; }

        public ICollection<string> Attributes { get; set; } = new List<string>();

        public ICollection<NetGenericParameter> GenericParameters { get; set; } = new List<NetGenericParameter>();

        public override string ToString()
        {
            return Name;
        }
    }


}