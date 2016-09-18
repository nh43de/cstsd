using System.Collections.Generic;
using System.Reflection;

namespace ToTypeScriptD.Core
{
    public class TsModule
    {
        public string Name { get; set; }

        public ICollection<TsNamespace> Namespaces { get; set; } = new List<TsNamespace>();

        public override string ToString()
        {
            return Name;
        }
    }
}
