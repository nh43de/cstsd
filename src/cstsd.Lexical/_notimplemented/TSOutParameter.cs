﻿//TODO: implement OutParameterReturnTypeWriter


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using ToTypeScriptD.Core.Config;
//using ToTypeScriptD.Core.Extensions;
//using ToTypeScriptD.Lexical.Extensions;


//namespace ToTypeScriptD.Lexical.WinMD
//{
//    public class OutParameterReturnTypeWriter 
//    {
//        private Type ReturnTypeReference;
//        private List<ParameterInfo> OutTypes;
//        private int IndentCount;
//        private Type TypeDefinition;
//        private string MethodName;
//        private ConfigBase config;

//        public OutParameterReturnTypeWriter(ConfigBase config, int indentCount, Type TypeDefinition, string methodName, Type retrunTypeReference, List<ParameterInfo> outTypes)
//        {
//            this.config = config;
//            this.IndentCount = indentCount;
//            this.TypeDefinition = TypeDefinition;
//            this.MethodName = methodName;
//            this.ReturnTypeReference = retrunTypeReference;
//            this.OutTypes = outTypes;
//        }

//        public string IndentValue
//        {
//            get { return config.Indent.Dup(IndentCount); }
//        }

//        public void Write(StringBuilder sb)
//        {
//            sb.AppendLine();
//            sb.Append(IndentValue); sb.AppendFormat("interface {0} {{{1}", TypeName, Environment.NewLine);

//            // return type
//            if (!(ReturnTypeReference.FullName == "System.Void"))
//            {
//                sb.AppendFormat("{0}{0}__returnValue: {1};{2}", IndentValue, ReturnTypeReference.ToTypeScriptTypeName(), Environment.NewLine);
//            }

//            // out parameter values
//            OutTypes.Each(item =>
//            {
//                sb.AppendFormat("{0}{0}{1}: {2};{3}", IndentValue, item.Name, item.ParameterType.ToTypeScriptTypeName(), Environment.NewLine);
//            });

//            sb.Append(IndentValue + "}" + Environment.NewLine);
//        }

//        public string TypeName
//        {
//            get
//            {
//                string genericParams = "";
//                if (TypeDefinition.GetGenericArguments().Any())
//                {
//                    genericParams = "<" + TypeDefinition.GetGenericArguments().Select(s => s.FullName).Join(", ") + ">";
//                }
//                return TypeDefinition.ToTypeScriptItemNameWinMD() + "_" + MethodName + "_OUT" + genericParams;
//            }
//        }

//        public string FullName
//        {
//            get
//            {
//                return TypeDefinition.Name + "." + TypeName;
//            }
//        }
//    }
//}
