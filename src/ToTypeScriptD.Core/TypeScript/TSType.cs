using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSGenericType : TSType
    {
        public ICollection<TSType> GenericParameters { get; set; } = new List<TSType>();

        public override string ToString()
        {
            return GenericParameters.Any() 
                ? $"{Name}<{string.Join(", ", GenericParameters.Select(p => p.ToString()))}>" 
                : Name;
        }

        public TSGenericType(string name, string nameSpace) : base(name, nameSpace)
        {
        }
    }

    public class TSType
    {
        public TSType(string name, string nameSpace)
        {
            Name = name;
        }

        public string Namespace { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}