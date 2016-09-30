using System.Collections.Generic;

namespace cstsd.Core.Net
{
    public class NetInterface : NetType
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

        public ICollection<NetMethod> Methods { get; set; } = new List<NetMethod>();
        public ICollection<NetField> Fields { get; set; } = new List<NetField>();
        public ICollection<NetType> BaseTypes { get; set; } = new List<NetType>();
        public ICollection<NetProperty> Properties { get; set; } = new List<NetProperty>();
        public ICollection<NetEvent> Events { get; set; } = new List<NetEvent>();
        
    }
}