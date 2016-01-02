using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSInterface : PrimaryTypeScriptType
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
        public ICollection<TSMethod> Methods { get; set; } = new List<TSMethod>();
        public ICollection<TSField> Fields { get; set; } = new List<TSField>();
        public ICollection<TSType> BaseTypes { get; set; } = new List<TSType>();
        public ICollection<TSType> GenericParameters { get; set; } = new List<TSType>();
        public ICollection<TSProperty> Properties { get; set; } = new List<TSProperty>();
        public ICollection<TSEvent> Events { get; set; } = new List<TSEvent>();
        
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



            return $"{exportStr}interface {Name}{generics}{extends}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   string.Join("\r\n", new[] { fields, properties, events, methods }.Where(s => !string.IsNullOrWhiteSpace(s))) +
                   @"}";
        }
    }
}