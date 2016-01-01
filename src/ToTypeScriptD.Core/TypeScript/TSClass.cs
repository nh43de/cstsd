using System;
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
        public ICollection<TSType> BaseTypes { get; set; } = new List<TSType>(); 

        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var methods = string.Join("\r\n\r\n", Methods.Select(m => m.ToString()));
            var fields = string.Join("\r\n", Fields.Select(f => f.ToString() + ";"));
            var extends = BaseTypes.Any() ? " extends " + string.Join(", ", BaseTypes.Select(b => b.ToString())) : "";

            return $"{exportStr}class {Name}{extends}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $"{fields.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"" + Environment.NewLine +
                   $"{methods.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"}";
        }
    }
}