using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class NetField : NetMember
    {
        public NetType FieldType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}