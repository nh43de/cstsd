using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetEnum : NetType
    {
        public HashSet<string> Enums { get; set; } = new HashSet<string>();

    }
}
