using System.Collections.Generic;

namespace cstsd.Core.Ts
{
    public class TsClass : TsInterface
    {
        public ICollection<TsType> NestedClasses { get; set; } = new List<TsType>(); 

    }
}