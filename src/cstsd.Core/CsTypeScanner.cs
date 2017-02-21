//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using cstsd.Core.Extensions;
//using cstsd.Core.Net;

//namespace cstsd.Core
//{
//    /// <summary>
//    /// Returns generation AST objects using reflection.
//    /// </summary>
//    public class CsTypeScanner //: ITypeScanner<Type>
//    {
//        #region assemblies

//        public Dictionary<string, NetAssembly> RegisteredAssemblies { get; set; } = new Dictionary<string, NetAssembly>();

        
//        public virtual NetAssembly RegisterAssembly(string assemblyPath)
//        {
//            var assembly = Assembly.LoadFrom(new FileInfo(assemblyPath).FullName);

//            return RegisterAssembly(assembly);
//        }

//        public virtual NetAssembly RegisterAssembly(Assembly assembly)
//        {
//            return RegisterAssembly(GetAssemblyTypes(assembly), assembly.FullName);
//        }

//        public virtual NetAssembly RegisterAssembly(Type[] types, string assemblyName)
//        {
//            var netAssembly = new NetAssembly {
//                Name = assemblyName
//            };
            
//            types = types.Where(t =>
//            {
//                try
//                {
//                    var ns = t.Namespace;

//                    return !string.IsNullOrWhiteSpace(ns);
//                }
//                catch (Exception)
//                {
//                    return false;
//                }
//            })
//            .ToArray();

//            var nses = types.Select(t => t.Namespace).Distinct().ToArray();

//            foreach (var ns in nses)
//            {
//                //start the scanning process
//                //TODO: this should be separated from this logic
//                netAssembly.Namespaces.Add(
//                    GetNamespace(ns,
//                       types.Where(t => t.Namespace == ns && t.IsNested == false)
//                           .OrderBy(t => t.Name)
//                           .ToArray())
//                );
//            }

//            RegisteredAssemblies.Add(assemblyName, netAssembly);

//            return netAssembly;
//        }

//        public static Type[] GetAssemblyTypes(Assembly assembly)
//        {
//            try
//            {
//                return assembly
//                    .ManifestModule
//                    .GetTypes()
//                    .ToArray();
//            }
//            catch (ReflectionTypeLoadException ex)
//            {
//                return ex.Types.Where(t => t != null).ToArray();
//            }
//        }

//        #endregion


//        #region types

//        public Dictionary<string, NetType> RegisteredTypes { get; set; } = new Dictionary<string, NetType>();

//        public virtual NetType RegisterType(Type td)
//        {
//            if (td == null)
//                return null;

//            if (td.IsGenericParameter)
//            {
//                return new NetType
//                {
//                    IsGenericParameter = true,
//                    Name = td.Name
//                };
//            }
            
//            if (!td.IsGenericParameter && string.IsNullOrEmpty(td.FullName))
//                throw new Exception($"Namespace '{td.FullName}' is not valid");

//            if (RegisteredTypes.ContainsKey(td.FullName))
//                return RegisteredTypes[td.FullName];

//            NetType nType;

//            if (td.IsEnum)
//            {
//                nType = RegisterEnum(td);
//            }
//            else if (td.IsInterface)
//            {
//                nType = RegisterInterface(td);
//            }
//            else if (td.IsClass)
//            {
//                if (td.BaseType != null
//                    && (td.BaseType.FullName == "System.MulticastDelegate"
//                        || td.BaseType.FullName == "System.Delegate"))
//                {
//                    nType = new NetType();
//                    //TODO: Delegate writer not implemented
//                    //return new DelegateWriter(td, indentCount, config, this);
//                }
//                else
//                {
//                    nType = RegisterClass(td);
//                }
//            }
//            else
//            {
//                nType = new NetType();

//                RegisteredTypes.Add(td.FullName, nType);
//            }

//            nType.Name = td.Name;
//            nType.FullName = td.FullName;
//            nType.Namespace = td.Namespace;
//            nType.IsPublic = td.IsPublic;
//            nType.ReflectedType = td;

//            nType.Attributes = GetCustomAttributes(td.CustomAttributes);
//            nType.GenericParameters = GetGenericParameters(td).ToArray();

//            return nType;
//        }

//        #endregion

//        public virtual ICollection<string> GetCustomAttributes(IEnumerable<CustomAttributeData> customAttributeData)
//        {
//            return customAttributeData.Select(a => a.AttributeType.Name).ToList();
//        }

//        public virtual NetNamespace GetNamespace(string namespaceStr, ICollection<Type> types)
//        {
//            var netNamespace = new NetNamespace
//            {
//                Name = namespaceStr
//            };
            
//            foreach (var td in types.Where(t => t.IsNested == false).OrderBy(t => t.Name))
//            {
//                netNamespace.TypeDeclarations.Add(RegisterType(td));
//            }

//            return netNamespace;
//        }

//        public virtual NetClass RegisterClass(Type td)
//        {
//            if (td == null)
//                return null;

//            var tsClass = new NetClass();
            
//            RegisteredTypes.Add(td.FullName, tsClass);
            
//            tsClass.BaseTypes = GetInheritedTypesAndInterfaces(td);
//            tsClass.Methods = GetMethods(td).ToArray();
//            tsClass.Fields = GetFields(td).ToArray();
//            tsClass.Properties = GetProperties(td).ToArray();
//            tsClass.Events = GetEvents(td).ToArray();
//            tsClass.NestedClasses = GetNestedTypes(td).ToArray();
            
//            return tsClass;
//        }

//        public virtual NetInterface RegisterInterface(Type td)
//        {
//            if (td == null)
//                return null;

//            var tsInterface = new NetInterface();
            
//            RegisteredTypes.Add(td.FullName, tsInterface);

//            tsInterface.BaseTypes = GetExportedInterfaces(td);
//            tsInterface.Methods = GetMethods(td).ToArray();
//            tsInterface.Fields = GetFields(td).ToArray();
//            tsInterface.Properties = GetProperties(td).ToArray();
//            tsInterface.Events = GetEvents(td).ToArray();

//            return tsInterface;
//        }


//        public virtual NetEnum RegisterEnum(Type td)
//        {
//            if (td == null)
//                return null;

//            var tsEnum = new NetEnum();
           
//            td.GetFields().For((item, i, isLast) =>
//            {
//                if (item.Name == "value__") return;

//                tsEnum.Enums.Add( new NetEnumValue
//                {
//                    Name = item.Name,
//                    EnumValue = Convert.ToInt32((object) item.GetRawConstantValue())
//                });
//            });

//            RegisteredTypes.Add(td.FullName, tsEnum);

//            return tsEnum;
//        }
        

//        public virtual IEnumerable<NetType> GetNestedTypes(Type td)
//        {
//            return
//                td?.GetNestedTypes()
//                    .Select(RegisterType);
//        } 


//        public virtual IEnumerable<NetGenericParameter> GetGenericParameters(Type td)
//        {
//            if (td == null)
//                yield break;

//            //generic arguments e.g. "T"
//            foreach (var genericParameter in td.GetGenericArguments())
//            {
//                var genParameter = new NetGenericParameter {
//                    Name = genericParameter.Name
//                }; //e.g. "T"

//                if(genericParameter.IsGenericParameter == false)
//                    continue;
               
//                //param constraints e.g. "where T : class" or "where T : ICollection<T>"
//                foreach(var constraint in genericParameter.GetGenericParameterConstraints())
//                {
//                    // Not sure how best to deal with multiple generic constraints (yet)
//                    // For now place in a comment
//                    // TODO: possible generate a new interface type that extends all of the constraints?
//                    genParameter.ParameterConstraints.Add(RegisterType(constraint));
//                }
                
//                yield return genParameter;
//            }
//        }


//        public IEnumerable<NetType> RegisterTypes(IEnumerable<Type> types)
//        {
//            foreach (var type in types)
//            {
//                yield return RegisterType(type);
//            }
//        }




//        public virtual List<NetType> GetInheritedTypesAndInterfaces(Type td)
//        {
//            if (td == null)
//                return null;

//            var rtn = GetExportedInterfaces(td);

//            if(td.BaseType != null)
//                rtn.Add(GetBaseType(td));

//            return rtn;
//        } 

//        public virtual NetType GetBaseType(Type td)
//        {
//            if (td == null)
//                return null;

//            NetType type = null;

//            //WriteExportedInterfaces(sb, inheriterString);
//            if (td.BaseType != null)
//            {
//                type = RegisterType(td.BaseType);
//            }

//            return type;
//        } 


//        public virtual List<NetType> GetExportedInterfaces(Type td) //TODO: rename
//        {
//            if (td == null)
//                return null;

//            var types = new List<NetType>();

//            //WriteExportedInterfaces(sb, inheriterString);
//            if (td.GetInterfaces().Any())
//            {
//                var interfaceTypes = td.GetInterfaces();
//                if (interfaceTypes.Any())
//                {
//                    foreach (var item in interfaceTypes)
//                    {
//                        types.Add(RegisterType(item));
//                    }
//                }
//            }

//            return types;
//        }
        
//        /// <summary>
//        /// Gets methods that a type has.
//        /// </summary>
//        /// <param name="td"></param>
//        /// <returns></returns>
//        public virtual IEnumerable<NetMethod> GetMethods(Type td)
//        {
//            if (td == null)
//                return null;

//            return td.GetMethods(
//                BindingFlags.Instance
//                | BindingFlags.Public
//                | BindingFlags.DeclaredOnly
//                | BindingFlags.NonPublic)
//                .Where(m => m.IsHideBySig == false)
//                .Select(GetMethod);
//        }

//        /// <summary>
//        /// Gets a method from method info.
//        /// </summary>
//        /// <param name="method"></param>
//        /// <returns></returns>
//        public virtual NetMethod GetMethod(MethodInfo method)
//        {
//            if (method == null)
//                return null;

//            var netMethod = new NetMethod
//            {
//                Name = method.Name,
//                IsStatic = method.IsStatic,
//                IsPublic = method.IsPublic,
//                IsConstructor = method.IsConstructor,
//                Parameters = GetParamters(method.GetParameters())?.ToArray(),
//                Attributes = GetCustomAttributes(method.CustomAttributes)
//            };
            
//            // constructors don't have return types.
//            if (!method.IsConstructor)
//            {
//                netMethod.ReturnType = RegisterType(method.ReturnType);
//            }

//            return netMethod;
//        }

//        /// <summary>
//        /// Gets function parameters
//        /// </summary>
//        /// <param name="parameters"></param>
//        /// <returns></returns>
//        public virtual IEnumerable<NetParameter> GetParamters(IEnumerable<ParameterInfo> parameters)
//        {
//            if (parameters == null)
//                return null;

//            return parameters.Select(parameter => new NetParameter
//            {
//                Name = parameter.Name,
//                IsOutParameter = parameter.IsOut,
//                FieldType = RegisterType(parameter.ParameterType),
//                Attributes = GetCustomAttributes(parameter.CustomAttributes)
//            });
//        }


//        public virtual IEnumerable<NetProperty> GetProperties(Type td)
//        {
//            if (td == null)
//                yield break;

//            foreach (var prop in td.GetProperties())
//            {
//                var propMethod = prop.GetMethod ?? prop.SetMethod;

//                var propType = prop.PropertyType;
                
//                var netProperty = new NetProperty
//                {
//                    IsStatic = propMethod.IsStatic,
//                    Name = prop.Name,
//                    SetterMethod = GetMethod(prop.GetSetMethod()),
//                    GetterMethod = GetMethod(prop.GetGetMethod()),
//                    Attributes = GetCustomAttributes(prop.CustomAttributes),
//                    FieldType = RegisterType(propType)
//                };

//                yield return netProperty;
//            }
//        }
        
//        public virtual IEnumerable<NetField> GetFields(Type td)
//        {
//            if(td == null)
//                return null;
            
//            return
//                td.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
//                    .Where(f => f.IsSpecialName == false)
//                    .Select(field => new NetField
//                    {
//                        IsPublic = field.IsPublic,
//                        Name = field.Name,
//                        FieldType = RegisterType(field.FieldType),
//                        Attributes = GetCustomAttributes(field.CustomAttributes)
//                    });
//        }

//        public virtual IEnumerable<NetEvent> GetEvents(Type td)
//        {
//            if (td == null)
//                return null;

//            return td.GetEvents().Select(eventInfo => new NetEvent
//            {
//                EventHandlerType = RegisterType(eventInfo.EventHandlerType),
//                Name = eventInfo.Name
//            });
//        }
//    }
//}
