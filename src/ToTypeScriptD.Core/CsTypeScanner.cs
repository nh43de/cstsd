using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ToTypeScriptD.Core.Extensions;

namespace ToTypeScriptD.Core
{
    /// <summary>
    /// Returns generation AST objects.
    /// </summary>
    public class CsTypeScanner //: ITypeScanner<Type>
    {
        #region assemblies

        public Dictionary<string, NetAssembly> RegisteredAssemblies { get; set; } = new Dictionary<string, NetAssembly>();

        
        public virtual NetAssembly RegisterAssembly(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(new FileInfo(assemblyPath).FullName);

            return RegisterAssembly(assembly);
        }

        public virtual NetAssembly RegisterAssembly(Assembly assembly)
        {
            return RegisterNetAssembly(GetAssemblyTypes(assembly), assembly.FullName);
        }

        public virtual NetAssembly RegisterNetAssembly(Type[] types, string assemblyName)
        {
            var netAssembly = new NetAssembly {
                Name = assemblyName
            };

            foreach (var ns in types.Select(t => t.Namespace).Distinct())
            {
                //start the scanning process
                //TODO: this should be separated from this logic
                netAssembly.Namespaces.Add(
                    GetNamespace(ns,
                       types.Where(t => t.Namespace == ns && t.IsNested == false)
                           .OrderBy(t => t.Name)
                           .ToArray())
                );
            }

            RegisteredAssemblies.Add(assemblyName, netAssembly);

            return netAssembly;
        }

        public virtual Type[] GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly
                    .ManifestModule
                    .GetTypes()
                    .ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).ToArray();
            }
        }

        #endregion


        #region types

        public Dictionary<string, NetType> RegisteredTypes { get; set; } = new Dictionary<string, NetType>();

        private NetType RegisterNetType(Type type, bool recursive = true)
        {
            if (string.IsNullOrEmpty(type.FullName))
                throw new Exception($"Namespace '{type.FullName}' is not valid");

            if (RegisteredTypes.ContainsKey(type.FullName))
                return RegisteredTypes[type.FullName];

            var netType = RegisterType(type);
            //RegisteredTypes.Add(type.FullName, netType);

            return netType;
        }

        #endregion
        
        public virtual NetNamespace GetNamespace(string namespaceStr, ICollection<Type> types)
        {
            var netNamespace = new NetNamespace
            {
                Namespace = namespaceStr
            };
            
            foreach (var td in types.Where(t => t.IsNested == false).OrderBy(t => t.Name))
            {
                netNamespace.TypeDeclarations.Add(RegisterNetType(td));
            }

            return netNamespace;
        }

        public virtual NetClass RegisterClass(Type td)
        {
            if (td == null)
                return null;

            var tsClass = new NetClass
            {
                Name = td.Name,
                IsPublic = td.IsPublic
            };
            
            RegisteredTypes.Add(td.FullName, tsClass);

            tsClass.GenericParameters = GetGenericParameters(td).ToArray();
            tsClass.BaseTypes = GetInheritedTypesAndInterfaces(td);
            tsClass.Methods = GetMethods(td).ToArray();
            tsClass.Fields = GetFields(td).ToArray();
            tsClass.Properties = GetProperties(td).ToArray();
            tsClass.Events = GetEvents(td).ToArray();
            tsClass.NestedClasses = GetNestedTypes(td).ToArray();
            
            return tsClass;
        }

        public virtual NetInterface RegisterInterface(Type td)
        {
            if (td == null)
                return null;

            var tsInterface = new NetInterface
            {
                Name = td.Name,
                IsPublic = td.IsPublic
            };

            RegisteredTypes.Add(td.FullName, tsInterface);

            tsInterface.GenericParameters = GetGenericParameters(td).ToArray();
            tsInterface.BaseTypes = GetExportedInterfaces(td);
            tsInterface.Methods = GetMethods(td).ToArray();
            tsInterface.Fields = GetFields(td).ToArray();
            tsInterface.Properties = GetProperties(td).ToArray();
            tsInterface.Events = GetEvents(td).ToArray();

            return tsInterface;
        }


        public virtual NetEnum RegisterEnum(Type td)
        {
            if (td == null)
                return null;

            var tsEnum = new NetEnum
            {
                Name = td.Name,
                IsPublic = td.IsPublic
            };
            
            td.GetFields().For((item, i, isLast) =>
            {
                if (item.Name == "value__") return;

                tsEnum.Enums.Add(item.Name);
            });

            RegisteredTypes.Add(td.FullName, tsEnum);

            return tsEnum;
        }
        

        public virtual IEnumerable<NetType> GetNestedTypes(Type td)
        {
            if (td == null)
                return null;

            return
                td.GetNestedTypes()
                    .Select(nt => RegisterNetType(nt));
        } 


        public virtual IEnumerable<NetGenericParameter> GetGenericParameters(Type td)
        {
            if (td == null)
                yield break;

            //generic arguments e.g. "T"
            foreach (var genericParameter in td.GetGenericArguments())
            {
                var genParameter = new NetGenericParameter {
                    Name = genericParameter.Name
                }; //e.g. "T"

                if(genericParameter.IsGenericParameter == false)
                    continue;
               
                //param constraints e.g. "where T : class" or "where T : ICollection<T>"
                foreach(var constraint in genericParameter.GetGenericParameterConstraints())
                {
                    // Not sure how best to deal with multiple generic constraints (yet)
                    // For now place in a comment
                    // TODO: possible generate a new interface type that extends all of the constraints?
                    genParameter.ParameterConstraints.Add(RegisterNetType(constraint));
                }
                
                yield return genParameter;
            }
        }


        public virtual NetType RegisterType(Type td)
        {
            if (td == null)
                return null;

            if (td.IsEnum)
            {
                return RegisterEnum(td);
            }
            if (td.IsInterface)
            {
                return RegisterInterface(td);
            }
            if (td.IsClass)
            {
                if (td.BaseType != null 
                    && (td.BaseType.FullName == "System.MulticastDelegate" 
                    || td.BaseType.FullName == "System.Delegate"))
                {
                    //TODO: Delegate writer not implemented
                    //return new DelegateWriter(td, indentCount, config, this);
                }
                else
                {
                    return RegisterClass(td);
                }
            }

            var t = new NetType
            {
                Name = td.Name,
                Namespace = td.Namespace,
                IsPublic = td.IsPublic
            };

            RegisteredTypes.Add(td.FullName, t);

            t.GenericParameters = GetGenericParameters(td).ToArray();

            return t;
        }


        public virtual List<NetType> GetInheritedTypesAndInterfaces(Type td)
        {
            if (td == null)
                return null;

            var rtn = GetExportedInterfaces(td);

            if(td.BaseType != null)
                rtn.Add(GetBaseType(td));

            return rtn;
        } 

        public virtual NetType GetBaseType(Type td)
        {
            if (td == null)
                return null;

            NetType type = null;

            //WriteExportedInterfaces(sb, inheriterString);
            if (td.BaseType != null)
            {
                type = RegisterNetType(td.BaseType);
            }

            return type;
        } 


        public virtual List<NetType> GetExportedInterfaces(Type td)
        {
            if (td == null)
                return null;

            var types = new List<NetType>();

            //WriteExportedInterfaces(sb, inheriterString);
            if (td.GetInterfaces().Any())
            {
                var interfaceTypes = td.GetInterfaces();
                if (interfaceTypes.Any())
                {
                    foreach (var item in interfaceTypes)
                    {
                        types.Add(RegisterNetType(item));
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
            if (td == null)
                return null;

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
            if (method == null)
                return null;

            var netMethod = new NetMethod
            {
                Name = method.Name,
                IsStatic = method.IsStatic,
                IsPublic = method.IsPublic,
                Parameters = GetParamters(method.GetParameters())?.ToArray()
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
            if (parameters == null)
                return null;

            return parameters.Select(parameter => new NetParameter
            {
                Name = parameter.Name,
                IsOutParameter = parameter.IsOut,
                Type = RegisterNetType(parameter.ParameterType)
            });
        }


        public virtual IEnumerable<NetProperty> GetProperties(Type td)
        {
            if (td == null)
                yield break;

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
            if(td == null)
                return null;
            
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
            if (td == null)
                return null;

            return td.GetEvents().Select(eventInfo => new NetEvent
            {
                EventHandlerType = RegisterNetType(eventInfo.EventHandlerType),
                Name = eventInfo.Name
            });
        }
    }
}
