using System.Collections.Generic;

namespace ToTypeScriptD.Core.Ts
{
    public class TsEnum : TsType
    {
        public ICollection<TsEnumValue> Enums { get; set; } = new List<TsEnumValue>();

    }
}
