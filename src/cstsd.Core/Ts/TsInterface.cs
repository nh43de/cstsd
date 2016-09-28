using System.Collections.Generic;

namespace ToTypeScriptD.Core.Ts
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
        public ICollection<TsType> BaseTypes { get; set; } = new List<TsType>();

        //class/interface members
        public ICollection<TsFunction> Methods { get; set; } = new List<TsFunction>();
        public ICollection<TsField> Fields { get; set; } = new List<TsField>();
        public ICollection<TsEvent> Events { get; set; } = new List<TsEvent>();
        
    }
}