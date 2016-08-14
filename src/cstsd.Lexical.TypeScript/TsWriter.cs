using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;

namespace cstsd.Lexical.TypeScript
{
    public class TsWriter // : INetWriter
    {
        private readonly TsWriterConfig _config;
        private readonly TextWriter _w;

        private string _indent => _config.Indent;


        private HashSet<string> _supportedNamespaces;


        public TsWriter(TsWriterConfig config, TextWriter w, IEnumerable<NetNamespace> supportedNamespaces)
        {
            _config = config;
            _w = w;
            _supportedNamespaces = new HashSet<string>(supportedNamespaces.Select(n => n.Name));
            _supportedNamespaces.Add("System");
            _supportedNamespaces.Add("System.Collections.Generic");
        }

        public virtual string WriteNamespace(NetNamespace netNamespace)
        {
            return WriteNamespace(netNamespace.Name, netNamespace.TypeDeclarations);
        }

        public virtual string WriteNamespace(string @namespace, IEnumerable<NetType> namespaceDeclarations)
        {
            var content = string.Join(_config.NewLines(2), namespaceDeclarations.Select(WriteType));
            
            return WriteNamespace(@namespace, content);
        }


        public virtual string WriteNamespace(NetNamespace netNamespace, string content)
        {
            return WriteNamespace(netNamespace.Name, content);
        }

        public virtual string WriteNamespace(string @namespace, string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
                content = content.Indent(_indent) + _config.NewLine;

            return $@"module {@namespace}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   content +
                   @"}";
        }

        public virtual string WriteType(NetType netType)
        {
            var @class = netType as NetClass;
            if (@class != null)
                return WriteClass(@class);

            var @enum = netType as NetEnum;
            if (@enum != null)
                return WriteEnum(@enum);

            var @interface = netType as NetInterface;
            if (@interface != null)
                return WriteInterface(@interface);

            throw new NotImplementedException();
        }

        
        public virtual string WriteTypeName(NetType netType)
        {
            return netType.ReflectedType.ToTypeScriptTypeName();
        }

        public virtual string WriteGenericParameterName(NetGenericParameter netGenericParameter)
        {
            return netGenericParameter.Name;
        }




        public virtual string WriteClass(NetClass netClass)
        {
            var exportStr = netClass.IsPublic ? "export " : "";
            var extends = netClass.BaseTypes.Any()
                ? " extends " + string.Join(", ", netClass.BaseTypes.Select(WriteTypeName))
                : "";
            var generics = netClass.GenericParameters.Any()
                ? $" <{string.Join(", ", netClass.GenericParameters.Select(WriteGenericParameter))}>"
                : "";

            var methods = GetMethodsString(netClass);
            var fields = GetFieldsString(netClass);
            var properties = GetPropertiesString(netClass);
            var events = GetEventsString(netClass);
            var nestedClasses = GetNestedClassesString(netClass);

            var body = JoinBodyText(fields, properties, events, methods);

            //TODO: config for brackets on same line as declaration
            return $"{exportStr}class {netClass.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   body +
                   @"}" +
                   nestedClasses;
        }

        public virtual string WriteInterface(NetInterface netInterface)
        {
            var exportStr = netInterface.IsPublic ? "export " : "";
            var extends = netInterface.BaseTypes.Any()
                ? " extends " + string.Join(", ", netInterface.BaseTypes.Select(WriteTypeName))
                : "";
            var generics = netInterface.GenericParameters.Any()
                ? $" <{string.Join(", ", netInterface.GenericParameters.Select(WriteGenericParameterName))}>"
                : "";

            var methods = GetMethodsString(netInterface);
            var fields = GetFieldsString(netInterface);
            var properties = GetPropertiesString(netInterface);
            var events = GetEventsString(netInterface);

            var body = JoinBodyText(fields, properties, events, methods);

            return $"{exportStr}interface {netInterface.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   body +
                   @"}";
        }


        public virtual string WriteEnum(NetEnum netEnum)
        {
            var enumStr = string.Join(","+_config.NewLine, netEnum.Enums).Indent(_config.Indent);

            return $"enum {netEnum.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $"{enumStr}" + _config.NewLine +
                   @"}";
        }

        //TODO: do this
        //public virtual string Write(NetGenericType netGenericType)
        //{
        //    return netGenericType.GenericParameters.Any()
        //        ? $"{netGenericType.Name}<{string.Join(", ", netGenericType.GenericParameters.Select(Write))}>"
        //        : netGenericType.Name;
        //}


        public virtual string WriteGenericParameter(NetGenericParameter netGenericParameter)
        {
            return netGenericParameter.ParameterConstraints.Any()
                ? $"{netGenericParameter.Name} extends {string.Join(", ", netGenericParameter.ParameterConstraints.Select(WriteTypeName))}"
                : $"{netGenericParameter.Name}";
        }




        public virtual string WriteMethod(NetMethod netMethod, string body)
        {
            var funParams = string.Join(", ", netMethod.Parameters.Select(p => WriteField(p, false)));
            var returnTypeStr = WriteTypeName(netMethod.ReturnType);
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;
            var exportStr = !netMethod.IsPublic ? "private " : "";
            var staticStr = netMethod.IsStatic ? "static " : "";

            return $"{exportStr}{staticStr}{netMethod.Name}({funParams}) : {returnType}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $@"{body.Indent(_config.Indent)}" + _config.NewLine +
                   @"}";
            //TODO: if body is null then we should only render a method signature
        }
        
        public virtual string WriteEvent(NetEvent netEvent)
        {
            //TODO: not exactly finished
            return $"<EVENT {netEvent.Name}>";
        }




        private string JoinBodyText(params string[] bodytexts)
        {
            var body = string.Join(_config.NewLines(2),
               bodytexts.Where(s => !string.IsNullOrWhiteSpace(s)));
            return body;
        }

        private string GetNestedClassesString(NetClass netClass)
        {
            var nestedClasses = string.Join(_config.NewLines(2), netClass.NestedClasses.Select(WriteType));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = _config.NewLines(2) + nestedClasses;
            return nestedClasses;
        }


        private string GetEventsString(NetInterface netInterface)
        {
            var events = string.Join(_config.NewLine, netInterface.Events.Select(p => WriteEvent(p) + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(_config.Indent) + _config.NewLine;
            return events;
        }

        private string GetPropertiesString(NetInterface netInterface)
        {
            var properties = string.Join(_config.NewLine,
                netInterface.Properties.Where(IsSupportedField)
                    .Select(p => WriteField(p, true) + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(_config.Indent) + _config.NewLine;
            return properties;
        }

        private string GetFieldsString(NetInterface netInterface)
        {
            var fields = string.Join(_config.NewLine,
                netInterface.Fields.Where(IsSupportedField)
                    .Select(f => WriteField(f, true) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_config.Indent) + _config.NewLine;
            return fields;
        }

        public virtual bool IsSupportedField(NetField netField)
        {
            var typeToCheck = netField.FieldType.ReflectedType;

            var arrayInfo = typeToCheck.GetTypeArrayInfo();
            if(arrayInfo.IsArrayType)
            {
                typeToCheck = arrayInfo.ReflectedType;
            }
            if (typeToCheck.IsNullable())
            {
                if (_supportedNamespaces.Contains(typeToCheck.GetNullableType().Namespace))
                {
                    return true;
                }
            }
            if (_supportedNamespaces.Contains(typeToCheck.Namespace))
            {
                return true;
            }
            return false;
        }



        public virtual string WriteField(NetField netField, bool useNullable)
        {
            var staticStr = netField.IsStatic ? "export " : "";
            var nullableStr = netField.FieldType.ReflectedType.IsNullable() ? "?" : "";

            return $"{staticStr}{netField.Name}{nullableStr} : {WriteTypeName(netField.FieldType)}";
        }

        private string GetMethodsString(NetInterface netInterface)
        {
            var methods = string.Join(_config.NewLines(2), netInterface.Methods.Select(m => WriteMethod(m, "func body")));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_config.Indent) + _config.NewLine;
            return methods;
        }
    }
}
