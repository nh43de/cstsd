using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.Net;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace ToTypeScriptD.Core
{
    /// <summary>
    /// Returns generation AST objects.
    /// </summary>
    public class RoslynTypeScanner //: ITypeScanner<Type>
    {
        public static void test(string path)
        {
            var a = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var cd = (ClassDeclarationSyntax)a.GetRoot().DescendantNodes().First(n => n is ClassDeclarationSyntax);
            
            return;
        }

        #region assemblies

        public RoslynTypeScanner(string assemblyName)
        {
            NetAssembly = new NetAssembly
            {
                Name = assemblyName
            };
        }

        public NetAssembly NetAssembly { get; set; }

        public virtual void RegisterCodeFile(string codeFilePath)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(codeFilePath));

            syntaxTree.GetRoot().ChildNodes().OfType<NamespaceDeclarationSyntax>().Each(RegisterNamespace);
        }

        public void RegisterNamespace(NamespaceDeclarationSyntax nsContext)
        {
            var nsName = nsContext.Name.ToString();
                
            var a = new NetNamespace
            {
                Name = nsName,
                TypeDeclarations = GetNamespaceTypeDeclarations(nsContext)
            };
            
            NetAssembly.Namespaces.Add(a);
        }

        public static NetType[] GetNamespaceTypeDeclarations(NamespaceDeclarationSyntax nsDeclarationSyntax)
        {
            var a = new List<NetType>();

            foreach (var cn in nsDeclarationSyntax.Members)
            {
                if (cn is ClassDeclarationSyntax)
                {
                    a.Add(GetNetClass((ClassDeclarationSyntax)cn));
                }
            }

            return a.ToArray();
        }
        
        public static NetClass GetNetClass(ClassDeclarationSyntax classDeclaration)
        {
            var name = classDeclaration.Identifier.ToString();
            
            var a = new NetClass
            {
                Attributes = GetAttributeList(classDeclaration.AttributeLists),
                IsPublic = IsPublic(classDeclaration.Modifiers),
                Name = classDeclaration.Identifier.ToString(),
                Methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>().Select(GetNetMethod).ToList(),
                Properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>().Select(GetNetProperty).ToList()
            };

            return a;
        }

        public static NetProperty GetNetProperty(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            return new NetProperty
            {
                Attributes = GetAttributeList(propertyDeclarationSyntax.AttributeLists),
                IsPublic = IsPublic(propertyDeclarationSyntax.Modifiers),
                Name = propertyDeclarationSyntax.Identifier.ToString(),
                IsStatic = IsStatic(propertyDeclarationSyntax.Modifiers),
                FieldType = GetType(propertyDeclarationSyntax.Type)
            };
        }


        public static List<string> GetAttributeList(SyntaxList<AttributeListSyntax> attributeListSyntax)
        {
            return attributeListSyntax.Select(als => als.Attributes.ToString()).ToList();
        }
        public static bool IsPublic(SyntaxTokenList syntaxTokenList)
        {
            return syntaxTokenList.Any(m => m.Kind() == SyntaxKind.PublicKeyword);
        }

        public static bool IsStatic(SyntaxTokenList syntaxTokenList)
        {
            return syntaxTokenList.Any(m => m.Kind() == SyntaxKind.StaticKeyword);
        }

        public static NetMethod GetNetMethod(MethodDeclarationSyntax methodDeclaration)
        {
            var item = new NetMethod
            {
                Name = methodDeclaration.Identifier.ToString(),
                Attributes = GetAttributeList(methodDeclaration.AttributeLists),
                IsConstructor = false, //constructor declarations are separate type of syntax
                IsStatic = IsStatic(methodDeclaration.Modifiers),
                IsPublic = IsPublic(methodDeclaration.Modifiers),
                Parameters = methodDeclaration.ParameterList.Parameters.Select(GetParameter).ToList(),
                ReturnType = GetType(methodDeclaration.ReturnType)
            };

            return item;
        }

        public static NetParameter GetParameter(ParameterSyntax parameterSyntax)
        {
            return new NetParameter
            { 
                Name = parameterSyntax.Identifier.ToString(),
                FieldType = GetType(parameterSyntax.Type)
            };
        }

        public static NetType GetType(TypeSyntax typeSyntax)
        {
            if (typeSyntax is GenericNameSyntax)
            {
                var genericType = (GenericNameSyntax)typeSyntax;

                return new NetType()
                {
                    Name = genericType.Identifier.ToString(),
                    GenericParameters = genericType.TypeArgumentList.Arguments.Select(ga => new NetGenericParameter
                    {
                        Name  = ga.ToString()
                    }).ToList()
                };
            }
            
            return new NetType
            {
                Name = typeSyntax.ToString()
            };
        }


        public static Type[] GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly
                    .ManifestModule
                    .GetTypes()
                    .ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).ToArray();
            }
        }

        #endregion

        

    }
}
