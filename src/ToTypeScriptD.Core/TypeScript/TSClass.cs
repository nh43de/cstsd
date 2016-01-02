using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSClass : TSInterface
    {

        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var methods = string.Join("\r\n\r\n", Methods.Select(m => m.ToString()));
            var fields = string.Join("\r\n", Fields.Select(f => f.ToString() + ";"));
            var extends = BaseTypes.Any() ? " extends " + string.Join(", ", BaseTypes.Select(b => b.ToString())) : "";
            var generics = GenericParameters.Any() ? $" <{string.Join(", ", GenericParameters.Select(g => g.ToString()))}>" : "";
            return $"{exportStr}class {Name}{generics}{extends}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $"{fields.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"" + Environment.NewLine +
                   $"{methods.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"}";
        }
    }
}