using System.IO;
using cstsd.TestAssembly.CSharp;

namespace cstsd.Tests.DotNet
{
    public class CSharpTestBase
    {
        public TestAssembly CSharpAssembly { get; private set; }
        public CSharpTestBase()
        {
            var path = typeof(IAmAnInterface).Assembly.Location;
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path));
            CSharpAssembly = new TestAssembly(path);
        }
    }
}
