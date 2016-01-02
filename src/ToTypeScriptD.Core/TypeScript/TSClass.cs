using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSClass : TSInterface
    {
        public ICollection<PrimaryTypeScriptType> NestedClasses { get; set; } = new List<PrimaryTypeScriptType>(); 

        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var extends = BaseTypes.Any() ? " extends " + string.Join(", ", BaseTypes.Select(b => b.ToString())) : "";
            var generics = GenericParameters.Any() ? $" <{string.Join(", ", GenericParameters.Select(g => g.ToString()))}>" : "";

            var methods = string.Join("\r\n\r\n", Methods.Select(m => m.ToString()));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;

            var fields = string.Join("\r\n", Fields.Select(f => f.ToString() + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;

            var properties = string.Join("\r\n", Properties.Select(p => p.ToString() + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;

            var events = string.Join("\r\n", Events.Select(p => p.ToString() + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;
            
            var nestedClasses = string.Join("\r\n\r\n", NestedClasses.Select(n => n.ToString()));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = nestedClasses.Indent(TSFormattingConfig.IndentSpaces) + Environment.NewLine;


            var body = string.Join("\r\n",
                new[] { fields, properties, events, methods, nestedClasses }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            return $"{exportStr}interface {Name}{generics}{extends}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   body +
                   @"}";
        }
    }
}