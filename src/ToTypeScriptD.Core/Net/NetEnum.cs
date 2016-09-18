using System.Collections.Generic;

namespace ToTypeScriptD.Core.Net
{
    public class NetEnum : NetType
    {
        public ICollection<NetEnumValue> Enums { get; set; } = new List<NetEnumValue>();

    }
}
