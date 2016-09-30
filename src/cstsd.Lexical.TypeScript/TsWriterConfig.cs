using cstsd.Lexical.Core;

namespace cstsd.TypeScript
{
    public class CstsdTask
    {
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Namespace { get; set; }
    }

    public class PocoTask : CstsdTask
    {

        public string[] SourceDirectories { get; set; }
        
        public string OutputName { get; set; }

        public bool Recursive { get; set; } = false;

    }

    public class ControllerTask : CstsdTask
    {
        public string SourceFile { get; set; }

    }


    public class TsWriterConfig
    {
        public bool CamelBackCase { get; set; } = true;
        
        public string NewLine { get; set; } = "\r\n";

        public IndentationFormatting IndentationFormatting { get; set; } = IndentationFormatting.SpaceX4;

        public string DefaultControllerNamespace { get; set; }

        public string DefaultPocoNamespace { get; set; }

        public ControllerTask[] ControllerTasks { get; set; }

        public PocoTask[] PocoObjectTasks { get; set; }



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
