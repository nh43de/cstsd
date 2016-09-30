using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsEnum : TsType
    {
        public ICollection<TsEnumValue> Enums { get; set; } = new List<TsEnumValue>();

    }
}
