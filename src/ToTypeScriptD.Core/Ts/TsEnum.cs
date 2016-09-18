using System.Collections.Generic;

namespace ToTypeScriptD.Core.Ts
{
    public class TsEnum : TsType
    {
        public HashSet<string> Enums { get; set; } = new HashSet<string>();

    }
}
