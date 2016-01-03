using System;

namespace ToTypeScriptD.Lexical.TypeScript
{
    public class TSEvent
    {
        public string Name { get; set; }
        public TSType EventHandlerType { get; set; }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}