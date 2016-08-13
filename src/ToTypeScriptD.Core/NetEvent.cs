using System;

namespace ToTypeScriptD.Core
{
    public class NetEvent
    {
        public string Name { get; set; }
        public NetType EventHandlerType { get; set; }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}