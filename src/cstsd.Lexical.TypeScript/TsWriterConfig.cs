using cstsd.Lexical.Core;

namespace cstsd.TypeScript
{
    public class TsWriterConfig
    {
        public bool CamelBackCase { get; set; } = true;
        
        public string NewLine { get; set; } = "\r\n";

        public IndentationFormatting IndentationFormatting { get; set; } = IndentationFormatting.SpaceX4;

        public string DefaultControllerNamespace { get; set; }

        public string DefaultPocoNamespace { get; set; }

        public string DefaultEnumNamespace { get; set; }

        public ControllerTask[] ControllerTasks { get; set; }

        public PocoTask[] PocoObjectTasks { get; set; }

        public EnumTask[] EnumTasks { get; set; }


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


    }
}
