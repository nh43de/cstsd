using System.IO;
using System.Linq;
using cstsd.Core.Extensions;
using cstsd.Core.Net;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cstsd.Core
{
    /// <summary>
    /// Returns generation AST objects.
    /// </summary>
    public sealed class RoslynTypeScanner //: ITypeScanner<Type>
    {
        public NetAssembly NetAssembly { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">Name to give the virtual assembly (for cstsd purposes).</param>
        public RoslynTypeScanner(string assemblyName)
        {
            NetAssembly = new NetAssembly
            {
                Name = assemblyName
            };
        }
        
        /// <summary>
        /// Load a .cs file into the virtual assembly.
        /// </summary>
        /// <param name="codeFilePath"></param>
        public void RegisterCodeFile(string codeFilePath)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(codeFilePath));

            syntaxTree.GetRoot().ChildNodes().OfType<NamespaceDeclarationSyntax>().Each(RegisterNamespace);
        }

        /// <summary>
        /// Registers a namespace (and all its declarations) into the assembly.
        /// </summary>
        /// <param name="nsContext"></param>
        public void RegisterNamespace(NamespaceDeclarationSyntax nsContext)
        {
            var nsName = nsContext.Name.ToString();
                
            var a = new NetNamespace
            {
                Name = nsName,
                TypeDeclarations = RoslynParserHelpers.GetNamespaceTypeDeclarations(nsContext)
            };
            
            NetAssembly.Namespaces.Add(a);
        }
    }
}
