using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cstsd.Core.Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cstsd.Core
{
    /// <summary>
    /// Helpers that convert Roslyn syntax into more generalized AST (Net* classes).
    /// </summary>
    public static class RoslynParserHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nsDeclarationSyntax"></param>
        /// <returns></returns>
        public static NetType[] GetNamespaceTypeDeclarations(NamespaceDeclarationSyntax nsDeclarationSyntax)
        {
            var a = new List<NetType>();

            foreach (var cn in nsDeclarationSyntax.Members)
            {
                if (cn is ClassDeclarationSyntax)
                {
                    a.Add(GetNetClass((ClassDeclarationSyntax)cn));
                }
                else if (cn is EnumDeclarationSyntax)
                {
                    a.Add(GetNetEnum((EnumDeclarationSyntax)cn));
                }
            }

            return a.ToArray();
        }

        public static NetEnum GetNetEnum(EnumDeclarationSyntax enumDeclaration)
        {
            var name = enumDeclaration.Identifier.ToString();

            var a = new NetEnum
            {
                Attributes = GetAttributeList(enumDeclaration.AttributeLists),
                IsPublic = IsPublic(enumDeclaration.Modifiers),
                Name = name,
                Enums = GetNetEnumValues(enumDeclaration.Members)
            };

            return a;
        }

        public static List<NetEnumValue> GetNetEnumValues(SeparatedSyntaxList<EnumMemberDeclarationSyntax> enumMembers)
        {
            return enumMembers.Select(m =>
            {
                var v = new NetEnumValue
                {
                    Name = m.Identifier.ToString()
                };

                int evi;
                if (m.EqualsValue?.Value != null && int.TryParse(m.EqualsValue.Value.ToString(), out evi))
                {
                    v.EnumValue = evi;
                }
                return v;
            }).ToList();
        }

        public static NetClass GetNetClass(ClassDeclarationSyntax classDeclaration)
        {
            var a = new NetClass
            {
                Attributes = GetAttributeList(classDeclaration.AttributeLists),
                IsPublic = IsPublic(classDeclaration.Modifiers),
                Name = classDeclaration.Identifier.ToString(),
                Methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>().Select(GetNetMethod).ToList(),
                Properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>().Select(GetNetProperty).ToList(),
                BaseTypes = GetBaseTypes(classDeclaration.BaseList),
                GenericParameters = GetGenericTypeParameters(classDeclaration.TypeParameterList)
            };

            return a;
        }

        public static List<NetGenericParameter> GetGenericTypeParameters(TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList == null)
                return new List<NetGenericParameter>();

            return typeParameterList.Parameters.Select(pa => new NetGenericParameter
            {
                Name = pa.Identifier.ToString()
            }).ToList();
        }

        public static List<NetType> GetBaseTypes(BaseListSyntax baseListSyntax)
        {
            if (baseListSyntax == null)
                return new List<NetType>();

            return baseListSyntax.Types.Select(bt => GetType(bt.Type)).ToList();
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
            if (typeSyntax is QualifiedNameSyntax)
            {
                typeSyntax = ((QualifiedNameSyntax)typeSyntax).Right;
            }

            if (typeSyntax is GenericNameSyntax)
            {
                var genericType = (GenericNameSyntax)typeSyntax;

                return new NetType()
                {
                    Name = genericType.Identifier.ToString(),
                    GenericParameters = GetGenericParameters(genericType.TypeArgumentList)
                };
            }

            if (typeSyntax is NullableTypeSyntax)
            {
                return new NetType
                {
                    Name = ((NullableTypeSyntax)typeSyntax).ElementType.ToString(),
                    IsNullable = true
                };
            }

            return new NetType
            {
                Name = typeSyntax.ToString(),
            };
        }

        public static List<NetGenericParameter> GetGenericParameters(TypeArgumentListSyntax typeArgumentList)
        {
            return typeArgumentList.Arguments.Select(typeSyntax =>
            {
                if (typeSyntax is GenericNameSyntax)
                {
                    return new NetGenericParameter
                    {
                        Name = ((GenericNameSyntax)typeSyntax).Identifier.ToString(),
                        NetGenericParameters = GetGenericParameters(((GenericNameSyntax)typeSyntax).TypeArgumentList)
                    };
                }
                else
                    return new NetGenericParameter
                    {
                        Name = typeSyntax.ToString()
                    };
            }).ToList();
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

    }
}