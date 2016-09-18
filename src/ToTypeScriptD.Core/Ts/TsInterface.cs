using System;
using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class TsInterface : TsType
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

        public ICollection<TsMethod> Methods { get; set; } = new List<TsMethod>();
        public ICollection<TsField> Fields { get; set; } = new List<TsField>();
        public ICollection<TsType> BaseTypes { get; set; } = new List<TsType>();
        public ICollection<TsProperty> Properties { get; set; } = new List<TsProperty>();
        public ICollection<TsEvent> Events { get; set; } = new List<TsEvent>();
        
    }
}