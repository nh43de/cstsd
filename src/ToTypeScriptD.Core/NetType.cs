using System;

namespace ToTypeScriptD.Core
{
    public class NetType
    {
        public NetType(string name, string nameSpace)
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