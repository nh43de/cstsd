using System.Text;
using ApprovalTests;
using ToTypeScriptD.Core;
using ToTypeScriptD.Lexical.DotNet;
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
        public void Test1()
        {
            var sb = new StringBuilder();
            var a = new ClassWriter(CSharpAssembly.AssemblyDefinition.GetType("ToTypeScriptD.TestAssembly.CSharp.GenericClass`1"), 4, new DotNetConfig());
            a.Write(sb);
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
