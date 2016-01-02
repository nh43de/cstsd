using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSInterface : TSModuleTypeDeclaration
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
        public ICollection<TSGenericParameter> GenericParameters { get; set; } = new List<TSGenericParameter>();
        public ICollection<TSProperty> Properties { get; set; } = new List<TSProperty>();
        public ICollection<TSEvent> Events { get; set; } = new List<TSEvent>();
        

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}