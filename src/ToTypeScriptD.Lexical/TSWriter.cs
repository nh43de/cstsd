using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Core.TypeScript;
using ToTypeScriptD.Core.TypeScript.Abstract;

namespace ToTypeScriptD.Lexical
{
    public class TSWriter
    {
        private readonly ConfigBase _config;
        private readonly TextWriter _w;

        public TSWriter(ConfigBase config, TextWriter w)
        {
            _config = config;
            _w = w;
        }

        public string Write(TSModule tsModule)
        {
            var interfaces = string.Join(_config.NewLines(2), tsModule.TypeDeclarations.Select(Write));
            if (!string.IsNullOrWhiteSpace(interfaces))
                interfaces = interfaces.Indent(_config.Indent) + _config.NewLine;

            return $@"module {tsModule.Namespace}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   interfaces +
                   @"}";
        }

        public string Write(TSModuleTypeDeclaration tsModuleTypeDeclaration)
        {
            var @class = tsModuleTypeDeclaration as TSClass;
            if (@class != null)
                return Write(@class);

            var @enum = tsModuleTypeDeclaration as TSEnum;
            if(@enum != null)
                return Write(@enum);

            var @interface = tsModuleTypeDeclaration as TSInterface;
            if(@interface != null)
                return Write(@interface);

            throw new NotImplementedException();
        }

        public string Write(TSEnum tsEnum)
        {
            var enumStr = string.Join(","+_config.NewLine, tsEnum.Enums);

            return $"enum {tsEnum.Name}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $"{enumStr}" + _config.NewLine +
                   @"}";
        }


        public string Write(TSGenericParameter tsGenericParameter)
        {
            return tsGenericParameter.ParameterConstraints.Any()
                ? $"{tsGenericParameter.Name} extends {string.Join(", ", tsGenericParameter.ParameterConstraints.Select(Write))}"
                : $"{tsGenericParameter.Name}";
        }


        public string Write(TSField tsField)
        {
            var staticStr = tsField.IsStatic ? "static " : "";
            return $"{staticStr}{tsField.Name} : {tsField.Type}";
        }

        public string Write(TSMethod tsMethod)
        {
            var funParams = string.Join(", ", tsMethod.Parameters.Select(Write));
            var returnTypeStr = Write(tsMethod.ReturnType);
            var returnType = string.IsNullOrWhiteSpace(returnTypeStr) ? "void" : returnTypeStr;
            var exportStr = tsMethod.IsExport ? "export " : "";
            var staticStr = tsMethod.IsStatic ? "static " : "";

            return $"{exportStr}{staticStr}{tsMethod.Name}({funParams}) : {returnType}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   $@"{tsMethod.Body.Indent(_config.Indent)}" + _config.NewLine +
                   @"}";
        }

        public string Write(TSGenericType tsGenericType)
        {
            return tsGenericType.GenericParameters.Any()
                ? $"{tsGenericType.Name}<{string.Join(", ", tsGenericType.GenericParameters.Select(Write))}>"
                : tsGenericType.Name;
        }

        public string Write(TSType tsType)
        {
            return tsType.Name;
        }

        public string Write(TSInterface tsInterface)
        {
            var exportStr = tsInterface.IsExport ? "export " : "";
            var extends = tsInterface.BaseTypes.Any() ? " extends " + string.Join(", ", tsInterface.BaseTypes.Select(Write)) : "";
            var generics = tsInterface.GenericParameters.Any() ? $" <{string.Join(", ", tsInterface.GenericParameters.Select(Write))}>" : "";


            var methods = string.Join(_config.NewLines(2), tsInterface.Methods.Select(Write));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_config.Indent) + _config.NewLine;

            var fields = string.Join(_config.NewLine, tsInterface.Fields.Select(f => Write(f) + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_config.Indent) + _config.NewLine;

            var properties = string.Join(_config.NewLine, tsInterface.Properties.Select(p => Write(p) + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(_config.Indent) + _config.NewLine;

            var events = string.Join(_config.NewLine, tsInterface.Events.Select(p => Write(p) + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(_config.Indent) + _config.NewLine;


            return $"{exportStr}interface {tsInterface.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   string.Join(_config.NewLine, new[] { fields, properties, events, methods }.Where(s => !string.IsNullOrWhiteSpace(s))) +
                   @"}";
        }

        public string Write(TSEvent tsEvent)
        {
            return $"<EVENT {tsEvent.Name}>";
        }

        public string Write(TSClass tsClass)
        {
            var exportStr = tsClass.IsExport ? "export " : "";
            var extends = tsClass.BaseTypes.Any() ? " extends " + string.Join(", ", tsClass.BaseTypes.Select(Write)) : "";
            var generics = tsClass.GenericParameters.Any() ? $" <{string.Join(", ", tsClass.GenericParameters.Select(Write))}>" : "";

            var methods = string.Join(_config.NewLines(2), tsClass.Methods.Select(m => m + ""));
            if (!string.IsNullOrWhiteSpace(methods))
                methods = methods.Indent(_config.Indent) + _config.NewLine;

            var fields = string.Join(_config.NewLine, tsClass.Fields.Select(f => f + ";"));
            if (!string.IsNullOrWhiteSpace(fields))
                fields = fields.Indent(_config.Indent) + _config.NewLine;

            var properties = string.Join(_config.NewLine, tsClass.Properties.Select(p => p + ";"));
            if (!string.IsNullOrWhiteSpace(properties))
                properties = properties.Indent(_config.Indent) + _config.NewLine;

            var events = string.Join(_config.NewLine, tsClass.Events.Select(p => p + ";"));
            if (!string.IsNullOrWhiteSpace(events))
                events = events.Indent(_config.Indent) + _config.NewLine;



            var body = string.Join(_config.NewLine,
                new[] { fields, properties, events, methods }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            var nestedClasses = string.Join(_config.NewLines(2), tsClass.NestedClasses.Select(n => n + ""));
            if (!string.IsNullOrWhiteSpace(nestedClasses))
                nestedClasses = _config.NewLines(2) + nestedClasses;

            //TODO: config for brackets on same line as declaration
            return $"{exportStr}class {tsClass.Name}{generics}{extends}" + _config.NewLine +
                   @"{" + _config.NewLine +
                   body +
                   @"}" +
                   nestedClasses;
        }
    }
}
