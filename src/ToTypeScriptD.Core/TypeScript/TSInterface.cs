using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSInterface
    {
        //TODO: interface/class events
        /*
        //if there are any events
        sb.AppendLine("addEventListener(eventName: string, listener: any): void;");
        sb.AppendLine("removeEventListener(eventName: string, listener: any): void;");

        //for each event
        var line = "addEventListener(eventName: \"{0}\", listener: {1}): void;".FormatWith(eventName, eventListenerType);
        line = "removeEventListener(eventName: \"{0}\", listener: {1}): void;".FormatWith(eventName, eventListenerType);
        line = "on{0}: (ev: {1}) => void;".FormatWith(eventName, eventListenerType);
        */

        public string Name { get; set; }
        public bool IsExport { get; set; }
        public ICollection<TSMethod> Methods { get; set; } = new List<TSMethod>();
        public ICollection<TSField> Fields { get; set; } = new List<TSField>();
        public ICollection<TSType> BaseTypes { get; set; } = new List<TSType>();
        public ICollection<TSType> GenericParameters { get; set; } = new List<TSType>();
        public ICollection<TSProperty> Properties { get; set; } = new List<TSProperty>();
        public ICollection<TSEvent> Events { get; set; } = new List<TSEvent>();
        
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