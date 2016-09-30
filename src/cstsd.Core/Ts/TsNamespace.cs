using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsNamespace
    {
        public string Name { get; set; }

        public ICollection<TsModule> Modules { get; set; } = new List<TsModule>();

        public ICollection<TsType> TypeDeclarations { get; set; } = new List<TsType>();

        public ICollection<TsFunction> FunctionDeclarations { get; set; } = new List<TsFunction>();


        public override string ToString()
        {
            return Name;
        }
    }
}
