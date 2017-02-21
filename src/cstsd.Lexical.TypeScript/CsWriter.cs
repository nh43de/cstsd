using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cstsd.Core.Extensions;
using cstsd.Core.Net;
using cstsd.Core.Ts;
using cstsd.Lexical.Core;

namespace cstsd.TypeScript
{
    public class CsWriter // : ITsWriter
    {
        private readonly WriterConfig _config;

        private string _indent => _config.IndentationFormatting.GetIndentationString();
        private TextWriter _w;
        private HashSet<string> _supportedNamespaces;

        public CsWriter(WriterConfig config, TextWriter w, IEnumerable<string> supportedNamespaces)
        {
            _config = config;
            _w = w;
            _supportedNamespaces = new HashSet<string>(supportedNamespaces)
            {
                "System",
                "System.Collections.Generic"
            };
        }

        public virtual string WriteNamespace(NetNamespace netModule)
        {
            var typeDeclarationsContent = string.Join(_config.NewLines(2), netModule.TypeDeclarations.Select(WriteType));

            var content = JoinNonEmpty(_config.NewLines(2), typeDeclarationsContent);

            if (!string.IsNullOrWhiteSpace(content))
                content = content.Indent(_indent) + _config.NewLine;

            return $@"namespace {netModule.Name}" + _config.NewLine +
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
        
        public virtual string WriteGenericParameterName(NetGenericParameter netGenericParameter)
        {
            return netGenericParameter.Name;
        }
        
        public virtual string WriteClass(NetClass netClass)
        {
            var exportStr = netClass.IsPublic ? "public " : "";
            var extends = netClass.BaseTypes.Any()
                ? " : " + string.Join(", ", netClass.BaseTypes.Select(WriteTypeName))
                : "";
            var generics = netClass.GenericParameters.Any()
                ? $"<{string.Join(", ", netClass.GenericParameters.Select(WriteGenericParameter))}>"
                : "";

            var methods = GetMethodsString(netClass);
            var fields = GetFieldsString(netClass);
            //var properties = GetPropertiesString(netClass);
            var events = GetEventsString(netClass);
            var nestedClasses = GetNestedClassesString(netClass);
            var constructor = netClass.Constructor != null ? GetConstructor(netClass) : "";
            //TODO: write constructor

            var body = JoinBodyText(fields, events, constructor, methods);

            //TODO: config for brackets on same line as declaration
            return $"{exportStr}class {netClass.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   body +
                   @"}" +
                   nestedClasses;
        }


        public virtual string WriteGenericArguments(ICollection<NetGenericParameter> genericParameters)
        {
            if (!genericParameters.Any()) return "";

            var genericParamsStr = string.Join(",", genericParameters.Select(gp => gp.Name + WriteGenericArguments(gp.NetGenericParameters)));

            return $"<{genericParamsStr}>";
        }

        public virtual string WriteInterface(NetInterface netInterface)
        {
            var exportStr = netInterface.IsPublic ? "public " : "";
            var extends = netInterface.BaseTypes.Any()
                ? " : " + string.Join(", ", netInterface.BaseTypes.Select(WriteTypeName))
                : "";
            var generics = netInterface.GenericParameters.Any()
                ? $"<{string.Join(", ", netInterface.GenericParameters.Select(WriteGenericParameterName))}>"
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


        public virtual string WriteEnum(NetEnum netEnum)
        {
            var enumStr = string.Join(","+_config.NewLine, netEnum.Enums.Select(WriteEnumValue)).Indent(_indent);
            var exportStr = netEnum.IsPublic ? "public " : "";

            return $"{exportStr}enum {netEnum.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $"{enumStr}" + _config.NewLine +
                   @"}";
        }

        public virtual string WriteEnumValue(NetEnumValue tsEnumValue)
        {
            if(tsEnumValue.EnumValue != null)
                return tsEnumValue.Name + " = " + tsEnumValue.EnumValue;

            return tsEnumValue.Name;
        }

        //TODO: do this
        //public virtual string Write(TsGenericType netGenericType)
        //{
        //    return netGenericType.GenericParameters.Any()
        //        ? $"{netGenericType.Name}<{string.Join(", ", netGenericType.GenericParameters.Select(Write))}>"
        //        : netGenericType.Name;
        //}

        public virtual string WriteGenericParameter(NetGenericParameter netGenericParameter)
        {
            return netGenericParameter.ParameterConstraints.Any()
                ? $"{netGenericParameter.Name} : {string.Join(", ", netGenericParameter.ParameterConstraints.Select(WriteTypeName))}"
                : $"{netGenericParameter.Name}";
        }
        
        public virtual string WriteEvent(NetEvent netEvent)
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

        private string GetFieldsString(NetInterface netInterface)
        {
            var fields = string.Join(_config.NewLine,
                netInterface.Fields.Select(f => WriteField(f) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_indent) + _config.NewLine;
            return fields;
        }
        
        public virtual string WriteMethod(NetMethod netMethod)
        {
            var funParams = string.Join(", ", netMethod.Parameters.Select(p => WriteField(p)));
            var returnTypeStr = WriteTypeName(netMethod.ReturnType);
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;

            var exportStr = !netMethod.IsPublic ? "private " : "public ";
            var staticStr = netMethod.IsStatic ? "static " : "";
            string accessModifier = $"{exportStr}{staticStr}";
            
            var body = netMethod.MethodBody;

            return $"{accessModifier}{returnType} {netMethod.Name}({funParams})" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $@"{body.Indent(_indent)}" + _config.NewLine +
                   @"}";
            //TODO: if body is null then we should only render a method signature?
        }

        public virtual string WriteTypeName(NetType netType)
        {
            if ((netType.GenericParameters?.Count ?? 0) == 0)
                return netType.Name;
           
            //render generic parameters
            return $"{netType.Name}{WriteGenericArguments(netType.GenericParameters)}";
        }

        private string GetMethodsString(NetInterface netInterface)
        {
            var methods = string.Join(_config.NewLines(2), netInterface.Methods.Select(m => WriteMethod(m)));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_indent) + _config.NewLine;
            return methods;
        }
        
        public virtual string GetConstructor(NetClass netClass)
        {
            var netConstructor = netClass.Constructor;

            var funParams = string.Join(", ", netConstructor.Parameters.Select(p => WriteField(p)));

            var exportStr = !netConstructor.IsPublic ? "private " : "public ";
            var staticStr = netConstructor.IsStatic ? "static " : "";
            string accessModifier = $"{exportStr}{staticStr}";

            var body = netConstructor.MethodBody;

            return $"{accessModifier}{netClass.Name}({funParams})" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $@"{body.Indent(_indent)}" + _config.NewLine +
                   @"}";
        }

        public virtual string GetFieldDeclarationTypeString(NetFieldDeclarationType fieldDeclarationType)
        {
            switch (fieldDeclarationType)
            {
                case NetFieldDeclarationType.Const:
                    return "const";
                case NetFieldDeclarationType.Var:
                    return "var";
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldDeclarationType), fieldDeclarationType, null);
            }
        }

        public virtual string WriteFieldDeclaration(NetFieldDeclaration tsFieldDeclaration)
        {
            var isPublic = tsFieldDeclaration.IsPublic ? "public " : "private ";
            var staticStr = tsFieldDeclaration.IsStatic ? "static " : "";
            var fieldDeclarationTypeString = GetFieldDeclarationTypeString(tsFieldDeclaration.FieldDeclarationType);
            var defaultValue = tsFieldDeclaration.DefaultValue;

            if (string.IsNullOrWhiteSpace(defaultValue))
            {
                defaultValue = "";
            }
            else
            {
                defaultValue = " = " + defaultValue + ";";
            }

            return $"{staticStr}{fieldDeclarationTypeString} {tsFieldDeclaration.Name}: {WriteTypeName(tsFieldDeclaration.FieldType)}{defaultValue}";
        }


        public virtual string WriteField(NetField netField)
        {
            var modStr = (netField is NetFieldDeclaration)
                ? GetFieldDeclarationTypeString(((NetFieldDeclaration) netField).FieldDeclarationType) + " "
                : "";
            var isPublic = netField.IsPublic ? "public " : "";
            var staticStr = netField.IsStatic ? "static " : "";
            var nullableStr = netField.FieldType.IsNullable ? "?" : "";
            var defaultValue = netField.DefaultValue;

            if (string.IsNullOrWhiteSpace(defaultValue))
            {
                defaultValue = "";
            }
            else
            {
                defaultValue = " = " + defaultValue + "";
            }

            return $"{isPublic}{modStr}{staticStr}{WriteTypeName(netField.FieldType)}{nullableStr} {netField.Name}{defaultValue}";
        }

        ///////////////////////


        

    }
}
