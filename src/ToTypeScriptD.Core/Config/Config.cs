using System.Collections.Generic;
using ToTypeScriptD.Core.Config;

namespace ToTypeScriptD.Core
{

    public abstract class ConfigBase
    {
        public abstract TypeWriters.ITypeWriterTypeSelector GetTypeWriterTypeSelector();
        public abstract bool CamelBackCase { get; set; }
        
        public bool IncludeSpecialTypes { get; set; }
        public string RegexFilter { get; set; } = "";
        public IndentationFormatting IndentationType { get; set; } = IndentationFormatting.SpaceX4;        
        public IEnumerable<string> AssemblyPaths { get; set; } = new string[0];

        public TypeWriters.ITypeNotFoundErrorHandler TypeNotFoundErrorHandler { get; set; }
            = new ConsoleErrorTypeNotFoundErrorHandler();


        public string Indent
        {
            get
            {
                switch (IndentationType)
                {
                    case IndentationFormatting.None: return "";
                    case IndentationFormatting.TabX1: return "\t";
                    case IndentationFormatting.TabX2: return "\t\t";
                    case IndentationFormatting.SpaceX1: return " ";
                    case IndentationFormatting.SpaceX2: return "  ";
                    case IndentationFormatting.SpaceX3: return "   ";
                    case IndentationFormatting.SpaceX4: return "    ";
                    case IndentationFormatting.SpaceX5: return "     ";
                    case IndentationFormatting.SpaceX6: return "      ";
                    case IndentationFormatting.SpaceX7: return "       ";
                    case IndentationFormatting.SpaceX8: return "        ";
                    default:
                        return "    ";
                }
            }
        }
    }
}
