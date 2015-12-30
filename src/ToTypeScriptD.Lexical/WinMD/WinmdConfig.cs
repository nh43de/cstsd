
using ToTypeScriptD.Core.Config;

namespace ToTypeScriptD.Lexical.WinMD
{
    public class WinmdConfig : ConfigBase
    {
        public override bool CamelBackCase
        {
            get { return true; }
            set { throw new System.NotSupportedException(); }
        }
    }
}
