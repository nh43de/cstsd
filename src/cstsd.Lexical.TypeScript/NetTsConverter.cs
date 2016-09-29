using System;
using System.Linq;
using cstsd.Lexical.TypeScript.Extensions;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Net;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class NetTsConverter //This is for pocos
    {
        public virtual TsType GetTsType(NetType netType) //needs to support case on tsclass, interface, enum, etc
        {
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
                Value = netEnumValue.Value
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
                IsNullable = IsFieldNullable(netField.FieldType),
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
                ParameterConstraints = netGenericParameter.ParameterConstraints.Select(GetTsType).ToList()
            };
        }

        public virtual string GetTsName(string name)
        {
            name = name.ToTypeScriptCase();
            return name;
        }
        
        public virtual string GetTsTypeName(NetType type)
        {
            var name = type.IsGenericParameter ? type.Name :
                type.ReflectedType == null 
                    ? type.ToTypeScriptTypeName()
                    : type.ReflectedType.ToTypeScriptTypeName();
            
            return name;
        }

    }
}