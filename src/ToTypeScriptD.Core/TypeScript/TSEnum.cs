using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSEnum : PrimaryTypeScriptType
    {
        public HashSet<string> Enums { get; set; } = new HashSet<string>();

        public override string ToString()
        {
            var enumStr = string.Join(",\r\n", Enums);

            return $"enum {Name}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $"{enumStr}" + Environment.NewLine +
                   @"}";
        }
    }
}
