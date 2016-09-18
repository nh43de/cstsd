using System.Linq;
using cstsd.Lexical.TypeScript.Extensions;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class NetTsConverter
    {
        public virtual TsEnum GetTsEnum(NetEnum netEnum)
        {
            return new TsEnum
            { 
                Enums = netEnum.Enums.Select(GetTsEnumValue).ToList(),
                IsPublic = netEnum.IsPublic,
                Name = GetTsName(netEnum.Name)
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

        public virtual TsInterface GetTsInterface(NetClass netClass)
        {
            return new TsInterface
            {
                IsPublic = netClass.IsPublic,
                Name = netClass.Name,
                Properties = netClass
                    .Properties
                    .Where(p => !p.Attributes.Contains("TsExcludeAttribute"))
                    .Select(GetTsProperty)
                    .ToList()
                //Fields = 
            };
        }

        public TsProperty GetTsProperty(NetProperty netProperty)
        {
            return new TsProperty
            {
                Name = GetTsName(netProperty.Name),
                FieldType = GetTsType(netProperty.FieldType),
                IsNullable = !netProperty.FieldType.IsGenericParameter && netProperty.FieldType.ReflectedType.IsNullable()
            };
        }

        public virtual TsType GetTsType(NetType netType)
        {
            return new TsType
            {
                Name = GetTsTypeName(netType),
                IsPublic = netType.IsPublic,
                GenericParameters = netType.GenericParameters.Select(GetTsGenericParameter).ToList()
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
            var name = type.IsGenericParameter ? type.Name : type.ReflectedType.ToTypeScriptTypeName();
            
            return name;
        }

    }
}