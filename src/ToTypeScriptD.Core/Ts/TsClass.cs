using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class TsClass : TsInterface
    {
        public ICollection<TsType> NestedClasses { get; set; } = new List<TsType>(); 

    }
}