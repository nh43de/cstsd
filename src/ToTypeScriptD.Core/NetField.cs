using System;

namespace ToTypeScriptD.Core
{
    public class NetField
    {
        public bool IsPublic { get; set; }
        public string Name { get; set; }
        public NetType Type { get; set; }
        public bool IsStatic { get; set; } 
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}