using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using cstsd.Lexical.TypeScript.Extensions;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class TsWriter // : ITsWriter
    {
        private readonly TsWriterConfig _config;

        private string _indent => _config.Indent;
        private TextWriter _w;
        private HashSet<string> _supportedNamespaces;
        
        public TsWriter(TsWriterConfig config, TextWriter w, IEnumerable<string> supportedNamespaces)
        {
            _config = config;
            _w = w;
            _supportedNamespaces = new HashSet<string>(supportedNamespaces)
            {
                "System",
                "System.Collections.Generic"
            };
        }

        public virtual string WriteNamespace(TsNamespace netNamespace, bool isTopLevelDeclareNamespace)
        {
            return WriteNamespace(netNamespace.Name, netNamespace.TypeDeclarations, isTopLevelDeclareNamespace);
        }

        public virtual string WriteNamespace(string @namespace, IEnumerable<TsType> namespaceDeclarations, bool isTopLevelDeclareNamespace)
        {
            var content = string.Join(_config.NewLines(2), namespaceDeclarations.Select(WriteType));
            
            return WriteNamespace(@namespace, content, isTopLevelDeclareNamespace);
        }
        
        public virtual string WriteNamespace(TsNamespace netNamespace, string content, bool isTopLevelDeclareNamespace)
        {
            return WriteNamespace(netNamespace.Name, content, isTopLevelDeclareNamespace);
        }

        public virtual string WriteNamespace(string @namespace, string content, bool isTopLevelDeclareNamespace)
        {
            if (!string.IsNullOrWhiteSpace(content))
                content = content.Indent(_indent) + _config.NewLine;

            var declare = isTopLevelDeclareNamespace ? "declare " : "";

            return $@"{declare}module {@namespace}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   content +
                   @"}";
        }

        public virtual string WriteType(TsType netType)
        {
            var @class = netType as TsClass;
            if (@class != null)
                return WriteClass(@class);

            var @enum = netType as TsEnum;
            if (@enum != null)
                return WriteEnum(@enum);

            var @interface = netType as TsInterface;
            if (@interface != null)
                return WriteInterface(@interface);

            throw new NotImplementedException();
        }
        
        public virtual string WriteGenericParameterName(TsGenericParameter netGenericParameter)
        {
            return netGenericParameter.Name;
        }
        
        public virtual string WriteClass(TsClass netClass)
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

        public virtual string WriteInterface(TsInterface netInterface)
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


        public virtual string WriteEnum(TsEnum netEnum)
        {
            var enumStr = string.Join(","+_config.NewLine, netEnum.Enums).Indent(_config.Indent);

            return $"enum {netEnum.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $"{enumStr}" + _config.NewLine +
                   @"}";
        }

        //TODO: do this
        //public virtual string Write(TsGenericType netGenericType)
        //{
        //    return netGenericType.GenericParameters.Any()
        //        ? $"{netGenericType.Name}<{string.Join(", ", netGenericType.GenericParameters.Select(Write))}>"
        //        : netGenericType.Name;
        //}

        public virtual string WriteGenericParameter(TsGenericParameter netGenericParameter)
        {
            return netGenericParameter.ParameterConstraints.Any()
                ? $"{netGenericParameter.Name} extends {string.Join(", ", netGenericParameter.ParameterConstraints.Select(WriteTypeName))}"
                : $"{netGenericParameter.Name}";
        }
        
        public virtual string WriteEvent(TsEvent netEvent)
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

        private string GetNestedClassesString(TsClass netClass)
        {
            var nestedClasses = string.Join(_config.NewLines(2), netClass.NestedClasses.Select(WriteType));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = _config.NewLines(2) + nestedClasses;
            return nestedClasses;
        }


        private string GetEventsString(TsInterface netInterface)
        {
            var events = string.Join(_config.NewLine, netInterface.Events.Select(p => WriteEvent(p) + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(_config.Indent) + _config.NewLine;
            return events;
        }

        private string GetPropertiesString(TsInterface netInterface)
        {
            var properties = string.Join(_config.NewLine,
                netInterface.Properties.Select(p => WriteField(p, true) + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(_config.Indent) + _config.NewLine;
            return properties;
        }

        private string GetFieldsString(TsInterface netInterface)
        {
            var fields = string.Join(_config.NewLine,
                netInterface.Fields.Select(f => WriteField(f, true) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_config.Indent) + _config.NewLine;
            return fields;
        }

        public virtual string WriteMethod(TsMethod netMethod, string body)
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

        public virtual string WriteTypeName(TsType netType)
        {
            return netType.Name;
        }

        private string GetMethodsString(TsInterface netInterface)
        {
            var methods = string.Join(_config.NewLines(2), netInterface.Methods.Select(m => WriteMethod(m, "func body")));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_config.Indent) + _config.NewLine;
            return methods;
        }
        public virtual string WriteField(TsField netField, bool useNullable)
        {
            var staticStr = netField.IsStatic ? "export " : "";
            var nullableStr = netField.IsNullable ? "?" : "";

            return $"{staticStr}{netField.Name}{nullableStr} : {WriteTypeName(netField.FieldType)}";
        }

        ///////////////////////


        

    }
}
