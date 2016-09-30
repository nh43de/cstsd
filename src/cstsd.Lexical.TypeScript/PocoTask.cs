namespace cstsd.TypeScript
{
    public class PocoTask : CstsdTask
    {

        public string[] SourceDirectories { get; set; }
        
        public string OutputName { get; set; }

        public bool Recursive { get; set; } = false;

    }
}