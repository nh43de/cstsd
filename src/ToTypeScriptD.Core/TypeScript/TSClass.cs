using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSClass
    {
        public string Name { get; set; }
        public bool IsExport { get; set; }
        public ICollection<TSMethod> Methods { get; set; } = new List<TSMethod>();
        public ICollection<TSField> Fields { get; set; } = new List<TSField>(); 
        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var methods = string.Join("\r\n\r\n", Methods.Select(m => m.ToString()));
            var fields = string.Join("\r\n", Fields.Select(f => f.ToString() + ";"));

            return $"{exportStr}class {Name}" +
                   @"{" +
                   $"{fields.Indent(TSFormattingConfig.IndentSpaces)}" +
                   @"" +
                   $"{methods.Indent(TSFormattingConfig.IndentSpaces)}" +
                   @"}";
        }
    }
}