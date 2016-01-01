using System;
using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.TypeScript
{
    public class TSInterface
    {
        public string Name { get; set; }
        public bool IsExport { get; set; }
        public ICollection<TSInterfaceMember> InterfaceMembers { get; set; } = new List<TSInterfaceMember>();
        public override string ToString()
        {
            var exportStr = IsExport ? "export " : "";
            var interfaceMembers = string.Join("\r\n", InterfaceMembers.Select(m => m.ToString() + ";"));
            return $"{exportStr}interface {Name}" + Environment.NewLine +
                   @"{" + Environment.NewLine +
                   $"{interfaceMembers.Indent(TSFormattingConfig.IndentSpaces)}" + Environment.NewLine +
                   @"}";
        }
    }
}