using cstsd.Tests.Helpers;
using Xunit;

namespace cstsd.Tests.DotNet
{

    public class CSharpTypeTests : CSharpTestBase
    {
        [Fact]
        public void GenerateFullAssembly()
        {
            var path = base.CSharpAssembly.ComponentPath;
            path.DumpDotNetAndVerify();
        }


        [Fact]
        public void UpperCasePropertyName()
        {
            var path = base.CSharpAssembly.ComponentPath;
            path.DumpDotNetAndVerify(config =>
            {
                config.CamelBackCase = false;
            });
        }
    }
}
