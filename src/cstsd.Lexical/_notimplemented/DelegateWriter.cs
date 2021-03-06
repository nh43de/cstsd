﻿//TODO: Implement delegate writer

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using ToTypeScriptD.Core.Config;
//using ToTypeScriptD.Core.Extensions;
//using ToTypeScriptD.Lexical.Extensions;
//using ToTypeScriptD.Lexical.TypeWriters;

//namespace ToTypeScriptD.Lexical.WinMD
//{
//    public class DelegateWriter : TypeWriterBase
//    {
//        public DelegateWriter(Type typeDefinition, int indentCount, ConfigBase config, ITypeWriterTypeSelector selector)
//            : base(typeDefinition, indentCount, config, selector)
//        {
//        }

//        public override void Write(System.Text.StringBuilder sb)
//        {
//            ++IndentCount;
//            Indent(sb); sb.AppendFormat("export interface {0}", TypeDefinition.ToTypeScriptItemNameWinMD());

//            if (TypeDefinition.GenericTypeArguments.Any())
//            {
//                sb.Append("<");
//                TypeDefinition.GetGenericArguments().For((genericParam, i, isLastItem) =>
//                {
//                    sb.AppendFormat("{0}{1}", genericParam.ToTypeScriptTypeName(), (isLastItem ? "" : ", "));
//                });
//                sb.Append(">");
//            }

//            sb.AppendLine(" {");
//            IndentCount++;

//            // 'ctor' is at index 0
//            // 'invoke' is at index 1
//            var invokeMethod = TypeDefinition.GetMethods()[1];
//            if (invokeMethod.GetParameters().Any())
//            {
//                var target = invokeMethod.GetParameters()[0];
//                Indent(sb); sb.AppendFormatLine("target: {0};", target.ParameterType.ToTypeScriptTypeName());
//            }
//            else
//            {
//                Indent(sb); sb.AppendFormatLine("target: any;");
//            }
//            Indent(sb); sb.AppendFormatLine("detail: any[];");

//            Indent(sb); sb.AppendLine("type: string;");
//            IndentCount--;
//            Indent(sb); sb.AppendLine("}");

//        }

//        private List<ITypeWriter> WriteMethods(StringBuilder sb)
//        {
//            List<ITypeWriter> extendedTypes = new List<ITypeWriter>();
//            var methodSignatures = new HashSet<string>();
//            foreach (var method in TypeDefinition.GetMethods())
//            {
//                var methodSb = new StringBuilder();

//                var methodName = method.Name;

//                // ignore special event handler methods
//                if (method.GetParameters().Any() &&
//                    method.GetParameters()[0].Name.StartsWith("__param0") &&
//                    (methodName.StartsWith("add_") || methodName.StartsWith("remove_")))
//                    continue;

//                //// already handled properties
//                //if (method.IsGetter || method.IsSetter)
//                //    continue;

//                // translate the constructor function
//                if (method.IsConstructor)
//                {
//                    continue;
//                }

//                // Lowercase first char of the method
//                methodName = methodName.ToTypeScriptName();

//                Indent(methodSb); Indent(methodSb);
//                if (method.IsStatic)
//                {
//                    methodSb.Append("static ");
//                }
//                methodSb.Append(methodName);

//                var outTypes = new List<ParameterInfo>();

//                methodSb.Append("(");
//                method.GetParameters().Where(w => w.IsOut).Each(e => outTypes.Add(e));
//                method.GetParameters().Where(w => !w.IsOut).For((parameter, i, isLast) =>
//                {
//                    methodSb.AppendFormat("{0}{1}: {2}{3}",
//                        (i == 0 ? "" : " "),                            // spacer
//                        parameter.Name,                                 // argument name
//                        parameter.ParameterType.ToTypeScriptTypeName(),     // type
//                        (isLast ? "" : ","));                           // last one gets a comma
//                });
//                methodSb.Append(")");

//                // constructors don't have return types.
//                if (!method.IsConstructor)
//                {
//                    string returnType;
//                    if (outTypes.Any())
//                    {
//                        var outWriter = new OutParameterReturnTypeWriter(Config, IndentCount, TypeDefinition, methodName, method.ReturnType, outTypes);
//                        extendedTypes.Add(outWriter);
//                        returnType = outWriter.TypeName;
//                    }
//                    else
//                    {
//                        returnType = method.ReturnType.ToTypeScriptTypeName();
//                    }

//                    methodSb.AppendFormat(": {0}", returnType);
//                }
//                methodSb.AppendLine(";");

//                var renderedMethod = methodSb.ToString();
//                if (!methodSignatures.Contains(renderedMethod))
//                    methodSignatures.Add(renderedMethod);
//            }

//            methodSignatures.Each(method => sb.Append(method));

//            return extendedTypes;
//        }
//    }
//}
