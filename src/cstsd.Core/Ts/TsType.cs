using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsType
    {
        public string Name { get; set; }

        public bool IsPublic { get; set; } = true; //TODO: rename to IsExport

        public bool IsArray { get; set; } = false;

        public ICollection<TsGenericParameter> GenericParameters { get; set; } = new List<TsGenericParameter>();

        public override string ToString()
        {
            return Name;
        }
    }


}