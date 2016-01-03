using System.Collections.Generic;

namespace ToTypeScriptD.Lexical.TypeScript
{
    public class TSAssembly
    {
        public TSAssembly(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        //TODO: TS Assembly will hold all modules and print as render.fromtypes (by namespaces)
        public ICollection<TSModule> Modules { get; set; } = new List<TSModule>();
    }
}
