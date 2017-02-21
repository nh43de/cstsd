using cstsd.Lexical.Core;

namespace cstsd.TypeScript
{
    public class WriterConfig
    {
        public bool CamelBackCase { get; set; } = true;
        
        public string NewLine { get; set; } = "\r\n";

        public IndentationFormatting IndentationFormatting { get; set; } = IndentationFormatting.SpaceX4;

        public string DefaultCsControllerNamespace { get; set; }
        public string DefaultTsControllerNamespace { get; set; }

        public string DefaultTsPocoNamespace { get; set; }

        public string DefaultTsEnumNamespace { get; set; }

        public ControllerTask[] ToCsControllerTasks { get; set; }

        public ControllerTask[] ToTsControllerTasks { get; set; }

        public PocoTask[] ToTsPocoObjectTasks { get; set; }

        public EnumTask[] ToTsEnumTasks { get; set; }


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
