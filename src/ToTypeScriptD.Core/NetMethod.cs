using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetMethod
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; } = false;

        public bool IsConstructor { get; set; } = false;
        public NetType ReturnType { get; set; }
        public string Body { get; set; } = ""; 
        public ICollection<NetParameter> Parameters { get; set; } = new List<NetParameter>();

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}