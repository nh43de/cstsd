using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSModule
    {
        public string Namespace { get; set; }
        public ICollection<PrimaryTypeScriptType> TypeDeclarations { get; set; } = new List<PrimaryTypeScriptType>();
        
        public override string ToString()
        {
            var interfaces = string.Join("\r\n\r\n", TypeDeclarations.Select(i => i.ToString()));
            if (!string.IsNullOrWhiteSpace(interfaces))
                interfaces = interfaces.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;
            
            return $@"module {Namespace}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   interfaces +
                   @"}";
        }
    }
}
