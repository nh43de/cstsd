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

        private string _indent => _config.IndentationFormatting.GetIndentationString();
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

        public virtual string WriteModule(TsModule netModule, bool isTopLevelDeclareNamespace)
        {
            var typeDeclarationsContent = string.Join(_config.NewLines(1), netModule.TypeDeclarations.Select(WriteType));
            var functionDeclarationsContent = string.Join(_config.NewLines(1), netModule.FunctionDeclarations.Select(fd => WriteMethod(fd, true)));
            var namespaceDeclarationsContent = string.Join(_config.NewLines(1), netModule.Namespaces.Select(ns => WriteNamespace(ns, false)));
            
            var content = JoinNonEmpty(typeDeclarationsContent, functionDeclarationsContent, namespaceDeclarationsContent);

            if (!string.IsNullOrWhiteSpace(content))
                content = content.Indent(_indent) + _config.NewLine;

            var declare = isTopLevelDeclareNamespace ? "declare " : "";
            var export = netModule.IsExport ? "export " : "";

            return $@"{declare}{export}module {netModule.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   content +
                   @"}";
        }

        public virtual string WriteNamespace(TsNamespace netModule, bool isTopLevelDeclareNamespace)
        {
            var typeDeclarationsContent = string.Join(_config.NewLines(1), netModule.TypeDeclarations.Select(WriteType));
            var functionDeclarationsContent = string.Join(_config.NewLines(1), netModule.FunctionDeclarations.Select(fd => WriteMethod(fd, true)));
            var namespaceDeclarationsContent = string.Join(_config.NewLines(1), netModule.Modules.Select(ns => WriteModule(ns, false)));

            var content = JoinBodyText(typeDeclarationsContent, functionDeclarationsContent, namespaceDeclarationsContent);

            if (!string.IsNullOrWhiteSpace(content))
                content = content.Indent(_indent) + _config.NewLine;

            var declare = isTopLevelDeclareNamespace ? "declare " : "";

            return $@"{declare}namespace {netModule.Name}" + _config.NewLine +
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
            //var properties = GetPropertiesString(netClass);
            var events = GetEventsString(netClass);
            var nestedClasses = GetNestedClassesString(netClass);

            var body = JoinBodyText(fields, events, methods);

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
            var events = GetEventsString(netInterface);

            var body = JoinBodyText(fields, events, methods);

            return $"{exportStr}interface {netInterface.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   body +
                   @"}";
        }


        public virtual string WriteEnum(TsEnum netEnum)
        {
            var enumStr = string.Join(","+_config.NewLine, netEnum.Enums.Select(WriteEnumValue)).Indent(_indent);

            return $"enum {netEnum.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $"{enumStr}" + _config.NewLine +
                   @"}";
        }

        public virtual string WriteEnumValue(TsEnumValue tsEnumValue)
        {
            return tsEnumValue.Name + " = " + tsEnumValue.Value;
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
            var body = JoinNonEmpty(_config.NewLines(2), bodytexts);
            return body;
        }

        private static string JoinNonEmpty(string joinStr, params string[] textStrings)
        {
            var content = string.Join(joinStr, textStrings.Where(p => !string.IsNullOrWhiteSpace(p)));
            return content;
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
                events = events.Indent(_indent) + _config.NewLine;
            return events;
        }

        //interfaces can't have properties
        //private string GetPropertiesString(TsInterface netInterface)
        //{
        //    var properties = string.Join(_config.NewLine,
        //        netInterface.Properties.Select(p => WriteField(p, p.IsNullable) + ";"));
        //    if (!string.IsNullOrWhiteSpace(properties))
        //        properties = properties.Indent(_config.Indent) + _config.NewLine;
        //    return properties;
        //}

        private string GetFieldsString(TsInterface netInterface)
        {
            var fields = string.Join(_config.NewLine,
                netInterface.Fields.Select(f => WriteField(f, f.IsNullable) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_indent) + _config.NewLine;
            return fields;
        }
        
        public virtual string WriteMethod(TsFunction netMethod, bool isGlobal)
        {
            var funParams = string.Join(", ", netMethod.Parameters.Select(p => WriteField(p, false)));
            var returnTypeStr = WriteTypeName(netMethod.ReturnType);
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;

            string accessModifier;
            if (isGlobal)
            {
                var exportStr = netMethod.IsPublic || netMethod.IsStatic ? "export " : "";
                accessModifier = $"{exportStr}function ";
            }
            else
            {
                var exportStr = !netMethod.IsPublic ? "private " : "";
                var staticStr = netMethod.IsStatic ? "static " : "";
                accessModifier = $"{exportStr}{staticStr}";
            }

            var body = netMethod.FunctionBody;

            return $"{accessModifier}{netMethod.Name}({funParams}): {returnType}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $@"{body.Indent(_indent)}" + _config.NewLine +
                   @"}";
            //TODO: if body is null then we should only render a method signature
        }

        public virtual string WriteTypeName(TsType netType)
        {
            if ((netType.GenericParameters?.Count ?? 0) == 0)
                return netType.Name;

            //render generic parameters
            return $"{netType.Name}<{string.Join(",", netType.GenericParameters.Select(gp => gp.Name))}>";
    }

        private string GetMethodsString(TsInterface netInterface)
        {
            var methods = string.Join(_config.NewLines(2), netInterface.Methods.Select(m => WriteMethod(m, false)));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_indent) + _config.NewLine;
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
