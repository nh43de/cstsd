using System.Collections.Generic;
using System.Linq;
using cstsd.Core.Net;
using cstsd.Core.Ts;

namespace cstsd.TypeScript
{
    public class NetTsPocoConverter : NetTsConverter
    {
        private static readonly HashSet<string> csValueTypes = new HashSet<string>
        {
            "bool",
            "byte",
            "char",
            "decimal",
            "double",
            "enum",
            "float",
            "int",
            "long",
            "sbyte",
            "short",
            "struct",
            "uint",
            "ulong",
            "ushort"
        };

        public TsInterface GetTsInterface(NetClass netClass)
        {
            return new TsInterface
            {
                IsPublic = netClass.IsPublic,
                GenericParameters = netClass.GenericParameters.Select(GetTsGenericParameter).ToList(),
                BaseTypes = netClass.BaseTypes.Select(GetTsType).ToList(),
                Name = netClass.Name,
                Fields = netClass
                    .Properties
                    .Where(p => !p.Attributes.Contains("TsIgnore"))
                    .Select(GetTsField)
                    .ToList()


                //Fields = 
            };
        }

        public override TsField GetTsField(NetField netField)
        {
            var f = base.GetTsField(netField);

            var isTypeValueType = csValueTypes.Contains(netField.FieldType.Name);
            var isTypeNullable = netField.FieldType.IsNullable;
            var isModelRequired = netField.Attributes.Any(a => a.Contains("Required"));

            //if it's a value type then we will always require it
            //if it's a nullable then we will always make it optional
            //otherwise, if it's model required then it will be required
            //otherwise, it's probably a reference type and will be optional

            if (isTypeNullable)
            {
                f.IsNullable = true;
            }
            else if (isModelRequired)
            {
                f.IsNullable = false;
            }
            else if(isTypeValueType)
            {
                f.IsNullable = false;
            }
            else
            {
                f.IsNullable = true;
            }

            return f;
        }
    }
}