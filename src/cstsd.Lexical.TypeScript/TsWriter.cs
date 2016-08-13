using System;
using System.IO;
using System.Linq;
using cstsd.Lexical.Core;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;

namespace cstsd.Lexical.TypeScript
{
    public class TsWriter : INetWriter
    {
        private readonly TsWriterConfig _config;
        private readonly TextWriter _w;

        private string _indent => _config.Indent;



        public TsWriter(TsWriterConfig config, TextWriter w)
        {
            _config = config;
            _w = w;
        }



        public virtual string Write(NetNamespace netNamespace)
        {
            var typeDeclarations = string.Join(_config.NewLines(2), netNamespace.TypeDeclarations.Select(Write));

            if (!string.IsNullOrWhiteSpace(typeDeclarations))
                typeDeclarations = typeDeclarations.Indent(_indent) + _config.NewLine;

            return $@"module {netNamespace.Namespace}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   typeDeclarations +
                   @"}";
        }

        public virtual string Write(NetType netType)
        {
            var @class = netType as NetClass;
            if (@class != null)
                return Write(@class);

            var @enum = netType as NetEnum;
            if(@enum != null)
                return Write(@enum);

            var @interface = netType as NetInterface;
            if(@interface != null)
                return Write(@interface);

            throw new NotImplementedException();
        }

        public virtual string Write(NetClass netClass)
        {
            var exportStr = netClass.IsPublic ? "export " : "";
            var extends = netClass.BaseTypes.Any() ? " extends " + string.Join(", ", netClass.BaseTypes.Select(Write)) : "";
            var generics = netClass.GenericParameters.Any() ? $" <{string.Join(", ", netClass.GenericParameters.Select(Write))}>" : "";

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

        public virtual string Write(NetInterface netInterface)
        {
            var exportStr = netInterface.IsPublic ? "export " : "";
            var extends = netInterface.BaseTypes.Any() ? " extends " + string.Join(", ", netInterface.BaseTypes.Select(Write)) : "";
            var generics = netInterface.GenericParameters.Any() ? $" <{string.Join(", ", netInterface.GenericParameters.Select(Write))}>" : "";

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


        public virtual string Write(NetEnum netEnum)
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


        public virtual string Write(NetGenericParameter netGenericParameter)
        {
            return netGenericParameter.ParameterConstraints.Any()
                ? $"{netGenericParameter.Name} extends {string.Join(", ", netGenericParameter.ParameterConstraints.Select(Write))}"
                : $"{netGenericParameter.Name}";
        }


        public virtual string Write(NetField netField)
        {
            var staticStr = netField.IsStatic ? "static " : "";
            return $"{staticStr}{netField.Name} : {Write(netField.Type)}";
        }

        public virtual string Write(NetMethod netMethod)
        {
            var funParams = string.Join(", ", netMethod.Parameters.Select(Write));
            var returnTypeStr = Write(netMethod.ReturnType);
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;
            var exportStr = !netMethod.IsPublic ? "private " : "";
            var staticStr = netMethod.IsStatic ? "static " : "";

            return $"{exportStr}{staticStr}{netMethod.Name}({funParams}) : {returnType}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   //TODO: body $@"{netMethod.Body.Indent(_config.Indent)}" + _config.NewLine +
                   @"}";
        }
        
        public virtual string Write(NetEvent netEvent)
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
            var nestedClasses = string.Join(_config.NewLines(2), netClass.NestedClasses.Select(Write));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = _config.NewLines(2) + nestedClasses;
            return nestedClasses;
        }


        private string GetEventsString(NetInterface netInterface)
        {
            var events = string.Join(_config.NewLine, netInterface.Events.Select(p => Write(p) + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(_config.Indent) + _config.NewLine;
            return events;
        }

        private string GetPropertiesString(NetInterface netInterface)
        {
            var properties = string.Join(_config.NewLine, netInterface.Properties.Select(p => Write(p) + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(_config.Indent) + _config.NewLine;
            return properties;
        }

        private string GetFieldsString(NetInterface netInterface)
        {
            var fields = string.Join(_config.NewLine, netInterface.Fields.Select(f => Write(f) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_config.Indent) + _config.NewLine;
            return fields;
        }

        private string GetMethodsString(NetInterface netInterface)
        {
            var methods = string.Join(_config.NewLines(2), netInterface.Methods.Select(Write));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_config.Indent) + _config.NewLine;
            return methods;
        }
    }
}
