using System.Linq;
using cstsd.Lexical.TypeScript.Extensions;
using ToTypeScriptD.Core;

namespace cstsd.Lexical.TypeScript
{
    public class NetTsConverter
    {
        public virtual TsClass GetTsClass(NetClass netClass)
        {
            return new TsClass
            {
                Name = netClass.Name,
                Properties = netClass.Properties.Select(netProperty => new TsProperty
                {
                    Name = GetTsName(netProperty.Name),
                    FieldType = GetTsType(netProperty.FieldType)
                }).ToList()
                //Fields = 
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
            var name = type.ReflectedType.ToTypeScriptTypeName();
            
            return name;
        }

    }
}