using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSClass : TSInterface
    {
        //TODO: replace \r\n to config 
        
        public ICollection<TSModuleTypeDeclaration> NestedClasses { get; set; } = new List<TSModuleTypeDeclaration>(); 

        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var extends = BaseTypes.Any() ? " extends " + string.Join(", ", BaseTypes.Select(b => b.ToString())) : "";
            var generics = GenericParameters.Any() ? $" <{string.Join(", ", GenericParameters.Select(g => g.ToString()))}>" : "";

            var methods = string.Join("\r\n\r\n", Methods.Select(m => m + ""));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(TSFormattingConfig.IndentSpaces) + "\r\n";

            var fields = string.Join("\r\n", Fields.Select(f => f + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(TSFormattingConfig.IndentSpaces) + "\r\n";

            var properties = string.Join("\r\n", Properties.Select(p => p + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(TSFormattingConfig.IndentSpaces) + "\r\n";

            var events = string.Join("\r\n", Events.Select(p => p + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(TSFormattingConfig.IndentSpaces) + "\r\n";

            

            var body = string.Join("\r\n",
                new[] { fields, properties, events, methods }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            var nestedClasses = string.Join("\r\n\r\n", NestedClasses.Select(n => n + ""));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = "\r\n\r\n" + nestedClasses;

            //TODO: config for brackets on same line as declaration
            return $"{exportStr}class {Name}{generics}{extends}" + "\r\n" +
                   @"{" + "\r\n" +
                   body +
                   @"}" +
                   nestedClasses;
        }
    }
}