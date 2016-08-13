using System;

namespace ToTypeScriptD.Core
{
    public class NetProperty : NetField
    {
        public NetMethod GetterMethod { get; set; }

        public NetMethod SetterMethod { get; set; }

    }
}
