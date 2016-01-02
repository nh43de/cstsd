using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSModule
    {
        public string Namespace { get; set; }
        public ICollection<TSInterface> Interfaces { get; set; } = new List<TSInterface>();
        public ICollection<TSClass> Clases { get; set; } = new List<TSClass>(); 
        public ICollection<TSEnum> Enums { get; set; } = new List<TSEnum>();

        public override string ToString()
        {
            var interfaces = string.Join("\r\n\r\n", Interfaces.Select(i => i.ToString()));
            if (!string.IsNullOrWhiteSpace(interfaces))
                interfaces = interfaces.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;

            var classes = string.Join("\r\r\n\n", Clases.Select(c => c.ToString()));
            if (!string.IsNullOrWhiteSpace(classes))
                classes = classes.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;
            
            return $@"module {Namespace}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   string.Join("\r\n", new[] { interfaces, classes }.Where(s => !string.IsNullOrWhiteSpace(s))) +
                   @"}";
        }
    }
}
