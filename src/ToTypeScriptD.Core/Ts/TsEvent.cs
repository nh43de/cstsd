using System;

namespace ToTypeScriptD.Core
{
    public class TsEvent
    {
        public string Name { get; set; }
        public TsType EventHandlerType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}