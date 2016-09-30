namespace cstsd.Tests.ExeTests
{
    public class ExeProcessResult
    {
        public string StdOut { get; set; }
        public int ExitCode { get; set; }

        public override string ToString()
        {
            return StdOut;
        }
    }
}
