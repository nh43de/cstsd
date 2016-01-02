using System.Collections.Generic;
using System.Linq;

namespace ToTypeScriptD.Core.Config
{

    public class TsdConfig
    {
        public virtual bool CamelBackCase { get; set; } = true;
        
        public bool IncludeSpecialTypes { get; set; }
        public string RegexFilter { get; set; } = "";
        public IndentationFormatting IndentationType { get; set; } = IndentationFormatting.SpaceX4;



        public bool RequireTypeScriptExportAttribute { get; set; } = true;

        public string NewLine { get; set; } = "\r\n";

        public string NewLines(int count)
        {
            var rtn = "";
            while (count > 0)
            {
                rtn += NewLine;
                count--;
            }
            return rtn;
        }

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
