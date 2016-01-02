using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSMethod
    {
        public string Name { get; set; }
        public bool IsExport { get; set; }
        public bool IsStatic { get; set; } = false;
        public TSType ReturnType { get; set; }
        public string Body { get; set; } = ""; 
        public ICollection<TSFuncParameter> Parameters { get; set; } = new List<TSFuncParameter>();

        public override string ToString()
        {
            var funParams = string.Join(", ", Parameters.Select(p => p.ToString()));
            var returnTypeStr = ReturnType.ToString();
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;
            var exportStr = IsExport ? "export " : "";
            var staticStr = IsStatic ? "static " : "";

            return $"{exportStr}{staticStr}{Name}({funParams}) : {returnType}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $@"{Body.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"}";
        }
    }
}