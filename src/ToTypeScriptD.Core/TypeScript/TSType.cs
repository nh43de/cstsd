using System;

namespace ToTypeScriptD.Core.TypeScript
{
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
            throw new NotImplementedException();
        }
    }
}