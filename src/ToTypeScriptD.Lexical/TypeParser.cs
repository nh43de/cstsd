using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.TypeScript;
using ToTypeScriptD.Lexical.DotNet;
using ToTypeScriptD.Lexical.Extensions;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD.Lexical
{
    /// <summary>
    /// Returns TS generation AST objects (TS* classes).
    /// </summary>
    public static class TypeParser
    {
        public static TSModule GetModule(string namespaceStr, ICollection<Type> types)
        {
            var tsModule = new TSModule
            {
                Namespace = namespaceStr
            };
            
            foreach (var td in types.Where(t => t.IsNested == false).OrderBy(t => t.Name))
            {

                if (td.IsEnum)
                {
                    tsModule.TypeDeclarations.Add(GetEnum(td));
                }
                else if (td.IsInterface)
                {
                    tsModule.TypeDeclarations.Add(GetInterface(td));
                }
                else if (td.IsClass)
                {
                    if (td.BaseType.FullName == "System.MulticastDelegate" ||
                        td.BaseType.FullName == "System.Delegate")
                    {
                        //TODO: not implemented
                        //return new DelegateWriter(td, indentCount, config, this);
                    }
                    else
                    {
                        tsModule.TypeDeclarations.Add(GetClass(td));
                    }
                }
            }

            return tsModule;
        }


        public static TSInterface GetInterface(Type td)
        {
            var tsInterface = new TSInterface
            {
                Name = td.ToTypeScriptItemNameWinMD(),
                GenericParameters = GetGenericConstraints(td),
                BaseTypes = GetExportedInterfaces(td),
                Methods = GetMethods(td),
                Fields = GetFields(td),
                Properties = GetProperties(td),
                Events = GetEvents(td)
            };
            return tsInterface;
        }

        public static TSClass GetClass(Type td)
        {
            var tsClass = new TSClass
            {
                Name = td.ToTypeScriptItemNameWinMD(),
                GenericParameters = GetGenericConstraints(td),
                BaseTypes = GetExportedInterfaces(td),
                Methods = GetMethods(td),
                Fields = GetFields(td),
                Properties = GetProperties(td),
                Events = GetEvents(td)
            };
            return tsClass;
        }
        public static TSEnum GetEnum(Type td)
        {
            var tsEnum = new TSEnum
            {
                Name = td.ToTypeScriptItemName()
            };

            td.GetFields().OrderBy(ob => ob.Name).For((item, i, isLast) => //.OrderBy(ob => ob.Name)
            {
                if (item.Name == "value__") return;

                tsEnum.Enums.Add(item.Name);
            });

            return tsEnum;
        }




        public static List<TSType> GetGenericConstraints(Type td)
        {
            var tsTypes = new List<TSType>();

            //generic constraints
            if (td.GetGenericArguments().Any())
            {
                td.GetGenericArguments().For((genericParameter, i, isLastItem) =>
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

        public static List<TSType> GetExportedInterfaces(Type td)
        {
            var types = new List<TSType>();

            //WriteExportedInterfaces(sb, inheriterString);
            if (td.GetInterfaces().Any())
            {
                var interfaceTypes = td.GetInterfaces().Where(w => !w.Name.ShouldIgnoreTypeByName());
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


        public static List<TSMethod> GetMethods(Type td)
        {
            var tsMethods = new List<TSMethod>();
            var methods =
                td.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly |
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
                    string returnType = method.ReturnType.ToTypeScriptTypeName();

                    //TODO: implement outtypes - returns an interface containing the out parameters
                    //if (outTypes.Any())
                    //{
                    //    //TODO: hook in outwriter
                    //    var outWriter = new OutParameterReturnTypeWriter(Config, IndentCount, td, methodName, method.ReturnType, outTypes);

                    //    //extendedTypes.Add(outWriter);
                    //    returnType = outWriter.TypeName;
                    //}

                    tsMethod.ReturnType = new TSType(returnType);
                }

                tsMethods.Add(tsMethod);
            }
            return tsMethods;
        }

        public static List<TSProperty> GetProperties(Type td)
        {
            var tsProperties = new List<TSProperty>();

            td.GetProperties().Each(prop =>
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



        public static List<TSField> GetFields(Type td)
        {
            var fields = new List<TSField>();

            td.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.IsSpecialName == false).Each(field =>
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

        public static List<TSEvent> GetEvents(Type td)
        {
            var events = new List<TSEvent>();

            if (td.GetEvents().Any())
            {
                td.GetEvents().For((item, i, isLast) =>
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
    }
}
