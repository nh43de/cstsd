using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToTypeScriptD.Core.Extensions;

namespace ToTypeScriptD.Core
{
    /// <summary>
    /// Returns generation AST objects.
    /// </summary>
    public class CsTypeScanner : ITypeScanner<Type>
    {
        public Dictionary<string, NetType> RegisteredNetTypes { get; set; } = new Dictionary<string, NetType>();

        public NetType RegisterNetType(Type type, bool recursive = true)
        {
            if (RegisteredNetTypes.ContainsKey(type.FullName))
                return RegisteredNetTypes[type.FullName];

            if (string.IsNullOrEmpty(type.Namespace))
                throw new Exception($"Namespace {type.Namespace} is not valid");

            var netType = GetType(type);
            RegisteredNetTypes.Add(type.Namespace, netType);

            return netType;
        }
        
        public Dictionary<string, NetAssembly> RegisteredNetAssemblies { get; set; } = new Dictionary<string, NetAssembly>();

        public NetType RegisterNetAssembly(Type type, bool recursive = true)
        {
            return null;
            //TODO: 
        }
 


        //TODO: need types external to assembly - and multiple files output? options?
        public virtual NetAssembly GetNetAssembly(ICollection<Type> types, string assemblyName)
        {
            var tsAssembly = new NetAssembly(assemblyName);

            foreach (var ns in types.Select(t => t.Namespace).Distinct())
            {
                 tsAssembly.Modules.Add(
                     GetModule(ns,
                        types.Where(t => t.Namespace == ns && t.IsNested == false)
                            .OrderBy(t => t.Name)
                            .ToArray()
                    )
                 );
            }

            return tsAssembly;
        }


        public virtual NetNamespace GetModule(string namespaceStr, ICollection<Type> types)
        {
            var tsModule = new NetNamespace
            {
                Namespace = namespaceStr
            };
            
            foreach (var td in types.Where(t => t.IsNested == false).OrderBy(t => t.Name))
            {
                tsModule.TypeDeclarations.Add(RegisterNetType(td));
            }

            return tsModule;
        }

        public virtual NetInterface GetInterface(Type td)
        {
            var tsInterface = new NetInterface
            {
                Name = td.Name,
                GenericParameters = GetGenericParameters(td).ToArray(),
                BaseTypes = GetExportedInterfaces(td),
                Methods = GetMethods(td).ToArray(),
                Fields = GetFields(td).ToArray(),
                Properties = GetProperties(td).ToArray(),
                Events = GetEvents(td).ToArray()
            };
            return tsInterface;
        }


        public virtual NetClass GetClass(Type td)
        {
            var tsClass = new NetClass
            {
                Name = td.Name,
                GenericParameters = GetGenericParameters(td).ToArray(),
                BaseTypes = GetInheritedTypesAndInterfaces(td),
                Methods = GetMethods(td).ToArray(),
                Fields = GetFields(td).ToArray(),
                Properties = GetProperties(td).ToArray(),
                Events = GetEvents(td).ToArray(),
                NestedClasses = GetNestedTypes(td).ToArray()
            };
            return tsClass;
        }


        public virtual NetEnum GetEnum(Type td)
        {
            var tsEnum = new NetEnum
            {
                Name = td.Name
            };

            td.GetFields().For((item, i, isLast) =>
            {
                if (item.Name == "value__") return;

                tsEnum.Enums.Add(item.Name);
            });

            return tsEnum;
        }
        

        public virtual IEnumerable<NetType> GetNestedTypes(Type td)
        {
            return
                td.GetNestedTypes()
                    .Select(nt => RegisterNetType(nt));
        } 


        public virtual IEnumerable<NetGenericParameter> GetGenericParameters(Type td)
        {
            //generic arguments e.g. "T"
            foreach(var genericParameter in td.GetGenericArguments())
            {
                var genParameter = new NetGenericParameter {
                    Name = genericParameter.Name
                }; //e.g. "T"

                //param constraints e.g. "where T : class" or "where T : ICollection<T>"
                foreach(var constraint in genericParameter.GetGenericParameterConstraints())
                {
                    // Not sure how best to deal with multiple generic constraints (yet)
                    // For now place in a comment
                    // TODO: possible generate a new interface type that extends all of the constraints?
                    genParameter.ParameterConstraints.Add(GetType(constraint));
                }
                
                yield return genParameter;
            }
        }

        public virtual NetType GetType(Type td)
        {
            return new NetType
            {
                Name = td.Name,
                Namespace = td.Namespace,
                GenericParameters = td.GetGenericArguments()
                                        .Select(ga => RegisterNetType(ga))
                                        .ToArray()
            };
        }


        public virtual List<NetType> GetInheritedTypesAndInterfaces(Type td)
        {
            var rtn = GetExportedInterfaces(td);

            if(td.BaseType != null)
                rtn.Add(GetBaseType(td));

            return rtn;
        } 

        public virtual NetType GetBaseType(Type td)
        {
            NetType type = null;

            //WriteExportedInterfaces(sb, inheriterString);
            if (td.BaseType != null)
            {
                type = (GetType(td.BaseType));
            }

            return type;
        } 


        public virtual List<NetType> GetExportedInterfaces(Type td)
        {
            var types = new List<NetType>();

            //WriteExportedInterfaces(sb, inheriterString);
            if (td.GetInterfaces().Any())
            {
                var interfaceTypes = td.GetInterfaces();
                if (interfaceTypes.Any())
                {
                    foreach (var item in interfaceTypes)
                    {
                        types.Add(GetType(item));
                    }
                }
            }

            return types;
        }
        
        /// <summary>
        /// Gets methods that a type has.
        /// </summary>
        /// <param name="td"></param>
        /// <returns></returns>
        public virtual IEnumerable<NetMethod> GetMethods(Type td)
        {
            return td.GetMethods(
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.DeclaredOnly
                | BindingFlags.NonPublic)
                .Where(m => m.IsHideBySig == false)
                .Select(GetMethod);
        }

        /// <summary>
        /// Gets a method from method info.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual NetMethod GetMethod(MethodInfo method)
        {
            var netMethod = new NetMethod
            {
                Name = method.Name,
                IsStatic = method.IsStatic,
                IsPublic = method.IsPublic,
                Parameters = GetParamters(method.GetParameters()).ToArray()
            };
            
            // constructors don't have return types.
            if (!method.IsConstructor)
            {
                netMethod.ReturnType = RegisterNetType(method.ReturnType);
            }

            return netMethod;
        }

        /// <summary>
        /// Gets function parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<NetParameter> GetParamters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameter => new NetParameter
            {
                Name = parameter.Name,
                IsOutParameter = parameter.IsOut,
                Type = RegisterNetType(parameter.ParameterType)
            });
        }


        public virtual IEnumerable<NetProperty> GetProperties(Type td)
        {
            foreach (var prop in td.GetProperties())
            {
                var propMethod = prop.GetMethod ?? prop.SetMethod;

                var propType = prop.PropertyType;

                var netProperty = new NetProperty
                {
                    IsStatic = propMethod.IsStatic,
                    Name = prop.Name,
                    SetterMethod = GetMethod(prop.GetSetMethod()),
                    GetterMethod = GetMethod(prop.GetGetMethod()),
                    Type = RegisterNetType(propType)
                };

                yield return netProperty;
            }
        }
        
        public virtual IEnumerable<NetField> GetFields(Type td)
        {
            return
                td.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => f.IsSpecialName == false)
                    .Select(field => new NetField
                    {
                        IsPublic = field.IsPublic,
                        Name = field.Name,
                        Type = RegisterNetType(field.FieldType)
                    });
        }

        public virtual IEnumerable<NetEvent> GetEvents(Type td)
        {
            return td.GetEvents().Select(eventInfo => new NetEvent
            {
                EventHandlerType = RegisterNetType(eventInfo.EventHandlerType),
                Name = eventInfo.Name
            });
        }
    }
}
