using System;

namespace ToTypeScriptD.Core
{
    public class NetProperty : NetField
    {
        public NetMethod GetterMethod { get; set; }

        public NetMethod SetterMethod { get; set; }
        
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
