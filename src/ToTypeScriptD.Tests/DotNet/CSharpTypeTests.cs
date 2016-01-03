using System.Text;
using ApprovalTests;
using ToTypeScriptD.Core;
using ToTypeScriptD.Lexical.DotNet;
using ToTypeScriptD.Tests.Helpers;
using Xunit;

namespace ToTypeScriptD.Tests.DotNet
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
