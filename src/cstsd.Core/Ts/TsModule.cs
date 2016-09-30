using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsModule
    {
        public string Name { get; set; }

        public ICollection<TsNamespace> Namespaces { get; set; } = new List<TsNamespace>();

        public ICollection<TsType> TypeDeclarations { get; set; } = new List<TsType>();

        public ICollection<TsFunction> FunctionDeclarations { get; set; } = new List<TsFunction>();

        public bool IsExport { get; set; } = false;

        public override string ToString()
        {
            return Name;
        }
    }
}
