using System;

namespace ToTypeScriptD.Core
{
    public class TsProperty : TsField
    {
        public TsMethod GetterMethod { get; set; }

        public TsMethod SetterMethod { get; set; }

    }
}
