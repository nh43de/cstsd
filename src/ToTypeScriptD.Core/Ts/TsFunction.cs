using System.Collections.Generic;

namespace ToTypeScriptD.Core.Ts
{
    public class TsFunction : TsMember
    {
        public bool IsConstructor { get; set; } = false;

        public TsType ReturnType { get; set; }

        public ICollection<TsParameter> Parameters { get; set; } = new List<TsParameter>();

        public override string ToString()
        {
            return Name;
        }
    }
}
