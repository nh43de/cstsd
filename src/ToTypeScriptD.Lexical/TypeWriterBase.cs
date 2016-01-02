using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToTypeScriptD.Core.Config;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.TypeScript;
using ToTypeScriptD.Lexical.DotNet;
using ToTypeScriptD.Lexical.Extensions;
using ToTypeScriptD.Lexical.TypeWriters;

namespace ToTypeScriptD.Lexical.WinMD
{
    public abstract class TypeWriterBase : ITypeWriter
    {
        public abstract void Write(System.Text.StringBuilder sb);

        public readonly Type TypeDefinition;
        public readonly ConfigBase Config;

        public ITypeWriterTypeSelector TypeSelector { get; private set; }
        

        public string FullName => TypeDefinition.Namespace + "." + TypeDefinition.ToTypeScriptItemName();
        public void Indent(StringBuilder sb) => sb.Append(IndentValue);
        protected int IndentCount;
        public string IndentValue => Config.Indent.Dup(IndentCount);




        protected TypeWriterBase(Type typeDefinition, int indentCount, ConfigBase config, ITypeWriterTypeSelector typeSelector)
        {
            this.TypeDefinition = typeDefinition;
            this.IndentCount = indentCount;
            this.Config = config;
            this.TypeSelector = typeSelector;
        }

        
        public TSClass GetClass()
        {
            var tsClass = new TSClass
            {
                Name = TypeDefinition.ToTypeScriptItemNameWinMD(),
                GenericParameters = GetGenericConstraints(),
                BaseTypes = GetExportedInterfaces(),
                Methods = GetMethods(),
                Fields = GetFields(),
                Properties = GetProperties(),
                Events = GetEvents()
            };
            return tsClass;
        }

        private List<TSType> GetGenericConstraints()
        {
            var tsTypes = new List<TSType>();

            //generic constraints
            if (TypeDefinition.GetGenericArguments().Any())
            {
                TypeDefinition.GetGenericArguments().For((genericParameter, i, isLastItem) =>
                {
                    genericParameter.GetGenericParameterConstraints().For((constraint, j, isLastItemJ) =>
                    {
                        // Not sure how best to deal with multiple generic constraints (yet)
                        // For now place in a comment
                        // TODO: possible generate a new interface type that extends all of the constraints?
                        tsTypes.Add(new TSType(constraint.ToTypeScriptTypeName()));
                    });
                });
            }

            return tsTypes;
        }

        private List<TSType> GetExportedInterfaces()
        {
            var types = new List<TSType>();

            //WriteExportedInterfaces(sb, inheriterString);
            if (TypeDefinition.GetInterfaces().Any())
            {
                var interfaceTypes = TypeDefinition.GetInterfaces().Where(w => !w.Name.ShouldIgnoreTypeByName());
                if (interfaceTypes.Any())
                {
                    foreach (var item in interfaceTypes)
                    {
                        types.Add(new TSType(item.ToTypeScriptTypeName()));
                    }
                }
            }

            return types;
        }


        private List<TSMethod> GetMethods()
        {
            var tsMethods = new List<TSMethod>();
            var methods =
                TypeDefinition.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly |
                                          BindingFlags.NonPublic).Where(m => m.IsHideBySig == false);

            foreach (var method in methods)
            {
                var tsMethod = new TSMethod();

                var methodName = method.Name;

                // ignore special event handler methods
                if (method.GetParameters().Any() &&
                    method.GetParameters()[0].Name.StartsWith("__param0") &&
                    (methodName.StartsWith("add_") || methodName.StartsWith("remove_")))
                    continue;

                if (method.IsSpecialName && !method.IsConstructor)
                    continue;

                // translate the constructor function
                if (method.IsConstructor)
                {
                    methodName = "constructor";
                }

                // Lowercase first char of the method
                methodName = methodName.ToTypeScriptName();

                tsMethod.IsStatic = method.IsStatic;
                tsMethod.Name = methodName;

                var outTypes = new List<ParameterInfo>();
                method.GetParameters().Where(w => w.IsOut).Each(e => outTypes.Add(e));
                method.GetParameters().Where(w => !w.IsOut).For((parameter, i, isLast) =>
                {
                    tsMethod.Parameters.Add(new TSFuncParameter
                    {
                        Name = parameter.Name,
                        Type = new TSType(parameter.ParameterType.ToTypeScriptTypeName())
                    });
                });

                // constructors don't have return types.
                if (!method.IsConstructor)
                {
                    string returnType;
                    if (outTypes.Any())
                    {
                        var outWriter = new OutParameterReturnTypeWriter(Config, IndentCount, TypeDefinition, methodName, method.ReturnType, outTypes);
                        
                        //extendedTypes.Add(outWriter);
                        returnType = outWriter.TypeName;
                    }
                    else
                    {
                        returnType = method.ReturnType.ToTypeScriptTypeName();
                    }

                    tsMethod.ReturnType = new TSType(returnType);
                }

                tsMethods.Add(tsMethod);
            }
            return tsMethods;
        }

        private List<TSProperty> GetProperties()
        {
            var tsProperties = new List<TSProperty>();

            TypeDefinition.GetProperties().Each(prop =>
            {
                var propName_ = prop.Name;
                var propName = propName_.ToTypeScriptName();
                
                var propMethod = prop.GetMethod ?? prop.SetMethod;

                var tsProperty = new TSProperty
                {
                    IsStatic = propMethod.IsStatic,
                    Name = propName,
                    Type = new TSType(prop.PropertyType.UnderlyingSystemType.ToTypeScriptTypeName())
                };

                tsProperties.Add(tsProperty);
            });

            return tsProperties;
        }

        public static TSEnum GetEnum(Type TypeDefinition)
        {
            var tsEnum = new TSEnum
            {
                Name = TypeDefinition.ToTypeScriptItemName()
            };

            TypeDefinition.GetFields().OrderBy(ob => ob.Name).For((item, i, isLast) => //.OrderBy(ob => ob.Name)
            {
                if (item.Name == "value__") return;

                tsEnum.Enums.Add(item.Name);
            });

            return tsEnum;
        }

        private List<TSField> GetFields()
        {
            var fields = new List<TSField>();

            TypeDefinition.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.IsSpecialName == false).Each(field =>
            {
                if (!field.IsPublic) return;
                var fieldName = field.Name.ToTypeScriptName();

                var tsField = new TSField
                {
                    Name = fieldName,
                    Type = new TSType(field.FieldType.ToTypeScriptType())
                };

                fields.Add(tsField);
            });

            return fields;
        }

        private List<TSEvent> GetEvents()
        {
            var events = new List<TSEvent>();

            if (TypeDefinition.GetEvents().Any())
            {
                TypeDefinition.GetEvents().For((item, i, isLast) =>
                {
                    events.Add(new TSEvent
                    {
                        EventHandlerType = new TSType(item.EventHandlerType.ToTypeScriptTypeName()),
                        Name = item.Name.ToLower()
                    });
                });
            }

            return events;
        }

        #region Promise Extension

        private void WriteAsyncPromiseMethods(StringBuilder sb)
        {
            string genericTypeArgName;
            if (IsTypeAsync(out genericTypeArgName))
            {
                sb.AppendLine();
                Indent(sb); Indent(sb); sb.AppendFormatLine("// Promise Extension");
                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => ToTypeScriptD.WinRT.IPromise<U>, error?: (error: any) => ToTypeScriptD.WinRT.IPromise<U>, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => ToTypeScriptD.WinRT.IPromise<U>, error?: (error: any) => U, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => U, error?: (error: any) => ToTypeScriptD.WinRT.IPromise<U>, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => U, error?: (error: any) => U, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("done<U>(success?: (value: {0}) => any, error?: (error: any) => any, progress?: (progress: any) => void): void;", genericTypeArgName);
            }
        }

        private bool IsTypeAsync(out string genericTypeArgName)
        {
            var currType = TypeDefinition;

            if (IsTypeAsync(TypeDefinition, out genericTypeArgName))
            {
                return true;
            }

            foreach (var i in TypeDefinition.GetInterfaces())
            {
                if (IsTypeAsync(i, out genericTypeArgName))
                {
                    return true;
                }
            }
            genericTypeArgName = "";
            return false;
        }

        private bool IsTypeAsync(Type typeReference, out string genericTypeArgName)
        {
            if (typeReference.FullName.StartsWith("Windows.Foundation.IAsyncOperation`1") ||
                typeReference.FullName.StartsWith("Windows.Foundation.IAsyncOperationWithProgress`2")
                )
            {
                var genericInstanceType = typeReference;// as GenericInstanceType;
                if (genericInstanceType == null)
                {
                    genericTypeArgName = "TResult";
                }
                else
                {
                    genericTypeArgName = genericInstanceType.GetGenericArguments()[0].ToTypeScriptTypeName();
                }
                return true;
            }

            genericTypeArgName = "";
            return false;
        }
        
        #endregion

        #region Array Extension
        private void WriteVectorArrayPrototypeExtensions(StringBuilder sb, bool wroteALengthProperty)
        {
            string genericTypeArgName;
            if (IsTypeArray(out genericTypeArgName))
            {
                sb.AppendLine();
                Indent(sb); Indent(sb); sb.AppendFormatLine("// Array.prototype extensions", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("toString(): string;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("toLocaleString(): string;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("concat(...items: {0}[][]): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("join(seperator: string): string;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("pop(): {0};", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("push(...items: {0}[]): void;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("reverse(): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("shift(): {0};", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("slice(start: number): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("slice(start: number, end: number): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("sort(): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("sort(compareFn: (a: {0}, b: {0}) => number): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("splice(start: number): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("splice(start: number, deleteCount: number, ...items: {0}[]): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("unshift(...items: {0}[]): number;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("lastIndexOf(searchElement: {0}): number;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("lastIndexOf(searchElement: {0}, fromIndex: number): number;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("every(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): boolean;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("every(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): boolean;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("some(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): boolean;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("some(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): boolean;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("forEach(callbackfn: (value: {0}, index: number, array: {0}[]) => void ): void;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("forEach(callbackfn: (value: {0}, index: number, array: {0}[]) => void , thisArg: any): void;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("map(callbackfn: (value: {0}, index: number, array: {0}[]) => any): any[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("map(callbackfn: (value: {0}, index: number, array: {0}[]) => any, thisArg: any): any[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("filter(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("filter(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): {0}[];", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("reduce(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any): any;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("reduce(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any, initialValue: any): any;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("reduceRight(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any): any;", genericTypeArgName);
                Indent(sb); Indent(sb); sb.AppendFormatLine("reduceRight(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any, initialValue: any): any;", genericTypeArgName);
                if (!wroteALengthProperty)
                {
                    Indent(sb); Indent(sb); sb.AppendFormatLine("length: number;", genericTypeArgName);
                }

            }
        }

        private bool IsTypeArray(out string genericTypeArgName)
        {
            var currType = TypeDefinition;

            if (IsTypeArray(TypeDefinition, out genericTypeArgName))
            {
                return true;
            }

            foreach (var i in TypeDefinition.GetInterfaces())
            {
                if (IsTypeArray(i, out genericTypeArgName))
                {
                    return true;
                }
            }
            genericTypeArgName = "";
            return false;
        }

        private bool IsTypeArray(Type typeReference, out string genericTypeArgName)
        {
            if (typeReference.FullName.StartsWith("Windows.Foundation.Collections.IVector`1") ||
                typeReference.FullName.StartsWith("Windows.Foundation.Collections.IVectorView`1")
                )
            {
                var genericInstanceType = typeReference; //as GenericInstanceType;
                if (genericInstanceType == null)
                {
                    genericTypeArgName = "T";
                }
                else
                {
                    genericTypeArgName = genericInstanceType.GetGenericArguments()[0].ToTypeScriptTypeName();
                }
                return true;
            }

            genericTypeArgName = "";
            return false;
        }

        #endregion


    }
    
}
