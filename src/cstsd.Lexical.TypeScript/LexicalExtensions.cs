using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;

namespace cstsd.Lexical.TypeScript
{
    public class NullableTypeInfo
    {
        public NetType BaseType { get; set; }

        public Type ReflectedBaseType { get; set; }

    }

    public class TypeArrayInfo
    {
        public bool IsArrayType { get; set; }
        public Type ReflectedType { get; set; }
    }

    public static class NullableTypeExtensions
    {

    }

    public static class EnumerableTypeExtensions
    {

    }

    public static class TypeHelperExtensions
    {
        /// <summary>
        /// If the given <paramref name="type"/> is an array or some other collection
        /// comprised of 0 or more instances of a "subtype", get that type
        /// </summary>
        /// <param name="type">the source type</param>
        /// <returns></returns>
        public static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            var elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            var elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
        }
    }

    public static class LexicalExtensions
    {
        //TODO: move this somewhere else?
        public static bool IsNullable(this Type Type)
        {
            var typeName = Type.Name;
            //var typeFullName = Type.FullName;

            return Type.Namespace == "System" && typeName == "Nullable`1";
        }

        public static Type GetNullableType(this Type Type)
        {
            if (!IsNullable(Type)) return Type;

            var genericInstanceType = Type;// as GenericInstanceType;
            if (genericInstanceType != null)
            {
                Type = genericInstanceType.GetGenericArguments()[0];
            }
            else
            {
                throw new NotImplementedException("For some reason this Nullable didn't have a generic parameter type? " + Type.FullName);
            }

            return Type;
        }
        public static TypeArrayInfo GetTypeArrayInfo(this Type td)
        {
            var enumeratedType = td.GetEnumeratedType();

            if (enumeratedType != null)
            {
                return new TypeArrayInfo
                {
                    IsArrayType = true,
                    ReflectedType = enumeratedType
                };
            }

            return new TypeArrayInfo
            {
                IsArrayType = false
            };
        }



        static readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>{
                { "System.String",               "string"},
                { "System.Type",                 "string /* System.Type? */"},
                { "System.Int16",                "number /* Int16 */"},
                { "System.Int32",                "number"},
                { "System.Int64",                "number"},
                { "System.UInt16",               "number /* UInt16 */"},
                { "System.UInt32",               "number"},
                { "System.UInt64",               "number"},
                { "System.Object",               "any"},
                { "Windows.Foundation.DateTime", "Date"},
                { "System.Void",                 "void"},
                { "System.Boolean",              "boolean"},
                { "System.IntPtr",               "number /* IntPtr */"},
                { "System.Byte",                 "any /* byte */"},
                { "System.Single",               "number"},
                { "System.Double",               "number"},
                { "System.Decimal",              "number"},
                { "System.Char",                 "any /* char */"}, 
                { "System.Guid",                 "any /* guid */"},
                { "System.Byte[]",               "any /* byte[] */"},
                { "System.Char[]",               "string"},
                { "System.DateTime",             "Date"}
        };

        static readonly Lazy<Dictionary<string, string>> _genericTypeMap = new Lazy<Dictionary<string, string>>(() => 
        {
            var a = _typeMap
                .ToDictionary(item => "<" + item.Key + ">", item => "<" + item.Value + ">");

            _typeMap
                .ToDictionary(item => item.Key + "[]", item => item.Value + "[]")
                    .Each(x => a.Add(x.Key, x.Value));

            return a;
        });



        //TODO: what?
        public static string ToTypeScriptItemName(this Type Type)
        {
            // Nested classes don't report their namespace. So we have to walk up the 
            // DeclaringType tree to find the root most type to grab it's namespace.
            var parentMostType = Type;
            while (parentMostType.DeclaringType != null)
            {
                parentMostType = parentMostType.DeclaringType;
            }

            var mainTypeName = Type.FullName;

            // trim namespace off of the front.
            mainTypeName = mainTypeName.Substring(parentMostType.Namespace.Length + 1);

            // replace the nested class slash with an underscore
            mainTypeName = mainTypeName.Replace("/", "_").StripGenericTick();

            return mainTypeName.StripGenericTick();
        }



        public static string GetArrayname(Type typeReference)
        {
            string fromName;

            Func<Type, bool> collectionInterfaces =
                i =>
                    (i.Name.Contains("ICollection") || i.Name.Contains("IEnumerable"));

            if (typeReference.GetInterfaces().Any(collectionInterfaces))
            {
                var iCollectionTypes = typeReference.GetInterfaces().Where(collectionInterfaces);
                var iCollectionGenertc = iCollectionTypes.FirstOrDefault(i => i.GetGenericArguments().Any()); //TODO: repeat getintfcs() - fix
                if (iCollectionGenertc == default(Type))
                {
                    fromName = $"System.Int32[]";
                }
                else
                {
                    fromName = $"{iCollectionGenertc.GetGenericArguments()[0].FullName ?? iCollectionGenertc.GetGenericArguments()[0].Name}[]";
                }
            }
            else
            {
                fromName = string.IsNullOrWhiteSpace(typeReference.FullName) ? typeReference.Name : typeReference.FullName;
            }

            return fromName;
        }


        //TODO: move this somewhere else?
        public static string ToTypeScriptTypeName(this Type typeReference)
        {
            typeReference = GetNullableType(typeReference);
            
            //TODO: a little hacky (the whole method)
            var fromName = GetArrayname(typeReference);

            // translate / in nested classes into underscores
            fromName = fromName.Replace("/", "_");

            // if we have a direct translation then use it
            if (_typeMap.ContainsKey(fromName))
            {
                return _typeMap[fromName];
            }

            // otherwise check for generic type mapping
            var genericType = _genericTypeMap.Value.FirstOrDefault(x => fromName.Contains(x.Key));
            if (!genericType.Equals(default(KeyValuePair<string, string>)))
            {
                fromName = fromName.Replace(genericType.Key, genericType.Value);
            }

            // If it's an array type return it as such.

            var arrayInfo = typeReference.GetTypeArrayInfo();

            if (arrayInfo.IsArrayType)
            {
                return arrayInfo.ReflectedType.ToTypeScriptTypeName() + "[]";
            }

            //if we still don't have it then ..
            fromName = fromName
                .StripGenericTick()
                .StripOutParamSymbol();
            
            _typeMap.Each(item =>
            {
                fromName = fromName.Replace(item.Key, item.Value);
            });

            return fromName;
        }





        //TODO: What does this do?
        public static Type GetTypeDefinition(Type Type)
        {
            if (Type == null)
                return null;

            try
            {
                var ass = Type.Assembly;

                var result = ass.Modules
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(td => td.FullName == Type.FullName);

                return result;
            }
            catch (Exception ex)
            {
                // for now ignore...
                // TODO: figure out a better way to handle non-framework assemblies...
            }

            return null;
        }

        public static bool ShouldIgnoreType(this Type name)
        {
            if (!name.IsPublic)
                return true;

            return false;
        }

        public static bool ShouldIgnoreTypeByName(this string name)
        {
            if (name == "<Module>")
                return true;

            if (name.StartsWith("__I") && name.EndsWith("PublicNonVirtuals"))
                return true;

            if (name.StartsWith("__I") && name.EndsWith("ProtectedNonVirtuals"))
                return true;

            return false;
        }


        static Dictionary<string, string> _specialEnumNames = new Dictionary<string, string>
        {
            {"GB2312", "gb2312"},
            {"PC437", "pc437"},
            {"NKo", "nko"},
        };

        // Copied and modified from Json.Net
        // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/StringUtils.cs
        public static string ToCamelCase(this string s, bool camelCaseConfig)
        {
            if (!camelCaseConfig)
                return s;

            if (_specialEnumNames.ContainsKey(s))
            {
                return _specialEnumNames[s];
            }

            if (String.IsNullOrEmpty(s))
                return s;

            if (!Char.IsUpper(s[0]))
                return s;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                bool hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || Char.IsUpper(s[i + 1]))
                {
                    char lowerCase;
#if !(NETFX_CORE || PORTABLE)
                    lowerCase = Char.ToLower(s[i], CultureInfo.InvariantCulture);
#else
                    lowerCase = char.ToLower(s[i]);
#endif

                    sb.Append(lowerCase);
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

        public static string ToTypeScript(this Type type, TsWriterConfig config = null)
        {
            if (config == null)
                config = new TsWriterConfig();

            throw new NotImplementedException();
        }

        // TODO: look to move this to the WinMDExtensions.cs
        public static string ToTypeScriptItemNameWinMD(this Type typeReference)
        {
            // Nested classes don't report their namespace. So we have to walk up the 
            // DeclaringType tree to find the root most type to grab it's namespace.
            var parentMostType = typeReference;
            while (parentMostType.DeclaringType != null)
            {
                parentMostType = parentMostType.DeclaringType;
            }

            var mainTypeName = typeReference.FullName;

            // trim namespace off of the front.
            mainTypeName = mainTypeName.Substring(parentMostType.Namespace.Length + 1);

            // replace the nested class slash with an underscore
            mainTypeName = mainTypeName.Replace("/", "_").Replace("+", "_").StripGenericTick();

            mainTypeName = mainTypeName.StripGenericTick();
            return mainTypeName;
        }


        public static string StripGenericTick(this string value)
        {
            4.Times().Each(x =>
            {
                value = value.Replace("`" + x, "");
            });
            return value;
        }

        public static string ToTypeScriptName(this string name)
        {
            if (name.ToUpper() == name)
            {
                return name.ToLower();
            }

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
        public static string StripOutParamSymbol(this string value)
        {
            return value.Replace("&", "");
        }

        public static void NewLine(this System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("");
        }
    }
}
