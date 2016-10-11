using System;
using System.Collections.Generic;
using System.Linq;
using cstsd.Core.Net;
using cstsd.Core.Ts;
using cstsd.TypeScript.Extensions;

namespace cstsd.TypeScript
{
    public class NetTsConverter //This is for pocos
    {
        public class NetCollectionType
        {
            public string Namespace { get; set; } = "";
            public string Name { get; set; }
        }
        
        private static readonly NetCollectionType[] NetCollectionTypes = new []
        {
            new NetCollectionType { Name = "ICollection", Namespace = "System.Collections.Generic"} ,
            new NetCollectionType { Name = "IList", Namespace = "System.Collections.Generic"} ,
            new NetCollectionType { Name = "List", Namespace = "System.Collections.Generic"} ,
            new NetCollectionType { Name = "ArrayList", Namespace = "System.Collections.Generic"} ,
            new NetCollectionType { Name = "IEnumerable", Namespace = "System.Collections.Generic"} ,
            new NetCollectionType { Name = "PaginatedList" }
        };

        public static readonly Lazy<HashSet<string>> NetCollectionTypesLazy = new Lazy<HashSet<string>>(() =>
        {
            var a = new HashSet<string>();
            foreach (var netCollectionType in NetCollectionTypes)
            {
                a.Add(netCollectionType.Namespace + "." + netCollectionType.Name);
                a.Add(netCollectionType.Name);
            }
            return a;
        });

        public virtual TsType GetTsType(NetType netType) //needs to support case on tsclass, interface, enum, etc
        {
            //TODO: this can probably be re-thought


            //    if (netType is NetEnum)
            //    {
            //        return GetTsEnum((NetEnum) netType);
            //    }

            //    if (netType is NetClass)
            //    {
            //        throw new NotImplementedException();
            //    }

            //    if (netType is NetInterface)
            //    {
            //        return new TsInterface
            //        {
            //            Name = GetTsTypeName(netType),
            //            IsPublic = netType.IsPublic,
            //            GenericParameters = netType.GenericParameters.Select(GetTsGenericParameter).ToList(),
            //            //TODO: more not implemented ...
            //        };
            //    }
            if (NetCollectionTypesLazy.Value.Contains(netType.Name))
            {
                return new TsType
                {
                    Name = TypeHelperExtensions.TryGetTsEquivalent(netType.GenericParameters.First().Name),
                    IsPublic = netType.IsPublic,
                    IsArray = true
                };
            }
            else if (netType.Name.EndsWith("[]"))
            {
                return new TsType
                {
                    Name = TypeHelperExtensions.TryGetTsEquivalent(netType.Name.Substring(0, netType.Name.Length - 2)),
                    IsPublic = netType.IsPublic,
                    IsArray = true
                };
            }

            return new TsType
            {
                Name = GetTsTypeName(netType),
                IsPublic = netType.IsPublic,
                GenericParameters = netType.GenericParameters.Select(GetTsGenericParameter).ToList()
            };
        }

        public virtual TsEnum GetTsEnum(NetEnum netEnum)
        {
            return new TsEnum
            { 
                Enums = netEnum.Enums.Select(GetTsEnumValue).ToList(),
                IsPublic = netEnum.IsPublic,
                Name = netEnum.Name
            };
        }

        public virtual TsEnumValue GetTsEnumValue(NetEnumValue netEnumValue)
        {
            return new TsEnumValue
            {
                Name = netEnumValue.Name,
                EnumValue = netEnumValue.EnumValue
            };
        }


        public virtual bool IsFieldNullable(NetType netType)
        {
            return !netType.IsGenericParameter && (netType.ReflectedType != null && netType.ReflectedType.IsNullable());
        }

        public virtual TsField GetTsField(NetField netField)
        {
            return new TsField
            {
                Name = GetTsName(netField.Name),
                FieldType = GetTsType(netField.FieldType),
                IsNullable = netField.FieldType.IsNullable,
                IsPublic = netField.IsPublic 
            };
        }

        public virtual TsFunction GetTsFunction(NetMethod netMethod)
        {
            return new TsFunction
            {
                IsConstructor = netMethod.IsConstructor,
                IsPublic = netMethod.IsPublic,
                IsStatic = netMethod.IsStatic,
                Name = GetTsName(netMethod.Name),
                Parameters = netMethod.Parameters.Select(GetTsParameter).ToList(),
                ReturnType = GetTsType(netMethod.ReturnType)
            };
        }

        public virtual TsParameter GetTsParameter(NetParameter netParameter)
        {
            return new TsParameter
            {
                Name = GetTsName(netParameter.Name),
                FieldType = GetTsType(netParameter.FieldType),
                IsNullable = IsFieldNullable(netParameter.FieldType)
            };
        }

        //using properties in typescript isn't very common 
        public virtual TsProperty GetTsProperty(NetProperty netProperty)
        {
            return new TsProperty
            {
                Name = GetTsName(netProperty.Name),
                FieldType = GetTsType(netProperty.FieldType),
                IsNullable = IsFieldNullable(netProperty.FieldType),
                IsPublic = netProperty.IsPublic
            };
        }
        
        public virtual TsGenericParameter GetTsGenericParameter(NetGenericParameter netGenericParameter)
        {
            return new TsGenericParameter
            {
                Name = netGenericParameter.Name,
                ParameterConstraints = netGenericParameter.ParameterConstraints.Select(GetTsType).ToList(),
                GenericParameters = netGenericParameter.NetGenericParameters.Select(GetTsGenericParameter).ToList()
            };
        }

        public virtual string GetTsName(string name)
        {
            name = name.ToTypeScriptCase();
            return name;
        }
        
        public virtual string GetTsTypeName(NetType type, bool writeGenerics = false)
        {
            var name = type.IsGenericParameter ? type.Name :
                type.ReflectedType == null 
                    ? type.ToTypeScriptTypeName(writeGenerics)
                    : type.ReflectedType.ToTypeScriptTypeName();
            
            return name;
        }

        
    }
}