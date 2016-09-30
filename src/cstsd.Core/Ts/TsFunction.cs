using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsFunction : TsMember
    {
        public bool IsConstructor { get; set; } = false;

        public TsType ReturnType { get; set; }

        public ICollection<TsParameter> Parameters { get; set; } = new List<TsParameter>();

        public string FunctionBody { get; set; } = "/* function body */";

        public override string ToString()
        {
            return Name;
        }
    }
}
