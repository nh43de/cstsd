using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSMethod
    {
        public string Name { get; set; }
        public bool IsExport { get; set; }
        public TSType ReturnType { get; set; }
        public string Body { get; set; } = ""; 
        public ICollection<TSFuncParameter> FunctionParameters { get; set; } = new List<TSFuncParameter>();

        public override string ToString()
        {
            var funParams = string.Join(", ", FunctionParameters.Select(p => p.ToString()));
            var returnTypeStr = ReturnType.ToString();
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;
            var exportStr = IsExport ? "export " : "";

            return $"{exportStr}{Name}({funParams}) : {returnType}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $@"{Body.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"}";
        }
    }
}