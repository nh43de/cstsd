using System;

namespace ToTypeScriptD.Core
{
    public class TsEvent : TsMember //TODO: not finished
    {
        public TsType EventHandlerType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}