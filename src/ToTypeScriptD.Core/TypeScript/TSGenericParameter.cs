using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSGenericParameter : TSType
    {

        public ICollection<TSType> ParameterConstraints { get; set; } = new List<TSType>();

        public override string ToString()
        {
            return ParameterConstraints.Any() 
                ? $"{Name} extends {string.Join(", ", ParameterConstraints.Select(c => c.ToString()))}" 
                : $"{Name}";
        }

        public TSGenericParameter(string name) : base(name, "")
        {
        }
    }
}