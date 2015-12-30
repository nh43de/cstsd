using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Lexical.Extensions;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.DotNet
{
    public abstract class TypeWriterBase : ITypeWriter
    {
        public readonly Type TypeDefinition;
        public readonly DotNetConfig Config;

        public ITypeWriterTypeSelector TypeSelector { get; private set; }

        public abstract void Write(StringBuilder sb);
        public virtual void Write(StringBuilder sb, Action midWrite) => midWrite();


        public string FullName => TypeDefinition.Namespace + "." + TypeDefinition.ToTypeScriptItemName();
        public void Indent(StringBuilder sb) => sb.Append(IndentValue);
        protected int IndentCount;
        public string IndentValue => Config.Indent.Dup(IndentCount);


        protected TypeWriterBase(Type typeDefinition, int indentCount, DotNetConfig config)
        {
            this.TypeDefinition = typeDefinition;
            this.IndentCount = indentCount;
            this.Config = config;
        }

        


        internal void WriteOutMethodSignatures(StringBuilder sb, string exportType, string inheriterString)
        {
            Indent(sb); sb.AppendFormat("export {0} {1}", exportType, TypeDefinition.ToTypeScriptItemName());
            WriteGenerics(sb);
            sb.Append(" ");
            WriteExportedInterfaces(sb, inheriterString);
            sb.AppendLine("{");

            var typeName = TypeDefinition.Name;

            WriteFields(sb);
            WriteProperties(sb);
            WriteInterfaceMethods(sb);

            Indent(sb); sb.AppendLine("}");

            WriteNestedTypes(sb);
        }


        private void WriteGenerics(StringBuilder sb)
        {
            if (TypeDefinition.ContainsGenericParameters)
            {
                sb.Append("<");
                TypeDefinition.GetGenericArguments().For((genericParameter, i, isLastItem) =>
                {
                    StringBuilder constraintsSB = new StringBuilder();
                    genericParameter.GetGenericParameterConstraints().For((constraint, j, isLastItemJ) =>
                    {
                        // Not sure how best to deal with multiple generic constraints (yet)
                        // For now place in a comment
                        // TODO: possible generate a new interface type that extends all of the constraints?
                        var isFirstItem = j == 0;
                        var isOnlyItem = isFirstItem && isLastItemJ;
                        if (isOnlyItem)
                        {
                            constraintsSB.AppendFormat(" extends {0}", constraint.ToTypeScriptType());
                        }
                        else
                        {
                            if (isFirstItem)
                            {
                                constraintsSB.AppendFormat(" extends {0} /*TODO:{1}", constraint.ToTypeScriptType(), (isLastItemJ ? "*/" : ", "));
                            }
                            else
                            {
                                constraintsSB.AppendFormat("{0}{1}", constraint.ToTypeScriptType(), (isLastItemJ ? "*/" : ", "));
                            }
                        }
                    });

                    sb.AppendFormat("{0}{1}{2}", genericParameter.ToTypeScriptType(), constraintsSB.ToString(), (isLastItem ? "" : ", "));
                });
                sb.Append(">");
            }
        }

        private static void WriteExtendedTypes(StringBuilder sb, List<ITypeWriter> extendedTypes)
        {
            extendedTypes.Each(item => item.Write(sb));
        }

        private void WriteNestedTypes(StringBuilder sb)
        {
            TypeDefinition.GetNestedTypes().Where(type => type.IsNestedPublic).Each(type =>
            {
                var typeWriter = TypeSelector.PickTypeWriter(type, IndentCount - 1, this.Config);
                sb.AppendLine();
                typeWriter.Write(sb);
            });
        }

        /// <summary>
        /// Writes properties of type in TS.
        /// </summary>
        /// <param name="sb"></param>
        private void WriteProperties(StringBuilder sb)
        {
            TypeDefinition.GetProperties(BindingFlags.DeclaredOnly & BindingFlags.Public).Each(prop =>
            {
                var propName = prop.Name.ToCamelCase(Config.CamelBackCase);
                Indent(sb); Indent(sb); sb.AppendFormat("{0}{1}: {2};", propName, prop.PropertyType.ToTypeScriptNullable(), prop.PropertyType.ToTypeScriptType());
                sb.AppendLine();
            });
        }

        private void WriteFields(StringBuilder sb)
        {
            TypeDefinition.GetFields().Each(field =>
            {
                if (!field.IsPublic) return;
                var fieldName = field.Name.ToCamelCase(Config.CamelBackCase);
                Indent(sb); Indent(sb); sb.AppendFormat("{0}: {1};", fieldName, field.FieldType.ToTypeScriptType());
                sb.AppendLine();
            });
        }
        
        private void WriteInterfaceMethods(StringBuilder sb)
        {
            if (TypeDefinition.IsInterface)
            {
                TypeDefinition.GetMethods().Each(method =>
                {
                    var methodName = method.Name.ToCamelCase(Config.CamelBackCase);
                    var returnType = method.ReturnType.ToTypeScriptType();
                    var methodParams = method.GetParameters().Select(param => $"{param.Name} : {param.ParameterType.ToTypeScriptType()}");

                    Indent(sb); Indent(sb);
                    sb.Append($"{methodName}({string.Join(",", methodParams)})");
                    if (!string.IsNullOrWhiteSpace(returnType))
                        sb.Append($" : {returnType}");

                    sb.Append(";");
                    sb.AppendLine();
                });
            }
        }

        //TODO: this is actually write base classes (ie. in ts, "extends [baseclass]")
        private void WriteExportedInterfaces(StringBuilder sb, string inheriterString)
        {
            //TODO: dbug only- remove
            var typeDefName = TypeDefinition.Name;

            if (TypeDefinition.GetInterfaces().Any() || 
                (TypeDefinition.BaseType != null && TypeDefinition.BaseType != typeof(object)))
            {
                var baseType = new Type[] {};

                if(TypeDefinition.BaseType != null)
                    baseType = new Type[] { TypeDefinition.BaseType };

                var interfaceTypes = baseType.Union(TypeDefinition.GetInterfaces())
                    .Where(w => !w.ShouldIgnoreTypeByName());
                
                if (interfaceTypes.Any())
                {
                    sb.Append(inheriterString);

                    var distinctTypes = interfaceTypes.Select(item => item.ToTypeScriptType()).Distinct();

                    distinctTypes.For((item, i, isLast) =>
                    {
                        sb.AppendFormat(" {0}{1}",item, isLast ? " " : ",");
                    });
                    
                }
            }
        }


    }
}
