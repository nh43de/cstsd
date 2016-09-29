
namespace cstsd.Lexical.Core
{
    public static class IndentationFormattingExtensions
    {
        public static string GetIndentationString(this IndentationFormatting indentationType)
        {
            switch (indentationType)
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
