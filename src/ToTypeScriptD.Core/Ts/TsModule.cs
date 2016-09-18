﻿using System.Collections.Generic;

namespace ToTypeScriptD.Core.Ts
{
    public class TsModule
    {
        public string Name { get; set; }

        public ICollection<TsNamespace> Namespaces { get; set; } = new List<TsNamespace>();

        public override string ToString()
        {
            return Name;
        }
    }
}
