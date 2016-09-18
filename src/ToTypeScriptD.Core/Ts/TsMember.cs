using System.Collections.Generic;

namespace ToTypeScriptD.Core
{
    public class TsMember
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; } = false;
    }
}