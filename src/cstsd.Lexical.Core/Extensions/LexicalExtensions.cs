using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ToTypeScriptD.Core;
using ToTypeScriptD.Lexical.Extensions;

namespace ToTypeScriptD.Lexical.WinMD
{
    public static class LexicalExtensions
    {

        private static bool IsNullable(this Type Type)
        {
            if (Type.IsValueType)
                return false;

            // TODO: is there a better way to determine if it's a Nullable?
            return Type.Namespace == "System" && Type.Name == "Nullable`1";
        }


        static Dictionary<string, string> typeMap = new Dictionary<string, string>{
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
                { "System.Char",                 "any /* char */"}, 
                { "System.Guid",                 "any /* guid */"},
                { "System.Byte[]",               "any /* byte[] */"},
                { "System.Char[]",               "string"},
                { "System.DateTime",             "Date"}
        };
        static Dictionary<string, string> genericTypeMap = null;

        public static string ToTypeScriptNullable(this Type Type)
        {
            return IsNullable(Type) ? "?" : "";
        }

        private static Type GetNullableType(Type Type)
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

        public static string ToTypeScriptType(this Type Type)
        {
            Type = GetNullableType(Type);

            if (genericTypeMap == null)
            {
                genericTypeMap = typeMap
                    .ToDictionary(item => "<" + item.Key + ">", item => "<" + item.Value + ">");

                typeMap
                    .ToDictionary(item => item.Key + "[]", item => item.Value + "[]")
                    .Each(x => genericTypeMap.Add(x.Key, x.Value));
            }

            var fromName = Type.FullName;

            // translate / in nested classes into underscores
            fromName = fromName.Replace("/", "_");

            if (typeMap.ContainsKey(fromName))
            {
                return typeMap[fromName];
            }

            var genericType = genericTypeMap.FirstOrDefault(x => fromName.Contains(x.Key));
            if (!genericType.Equals(default(System.Collections.Generic.KeyValuePair<string, string>)))
            {
                fromName = fromName.Replace(genericType.Key, genericType.Value);
            }

            fromName = fromName
                .StripGenericTick();

            // To lazy to figure out the Mono.Cecil way (or if there is a way), but do 
            // some string search/replace on types for example:
            //
            // turn
            //      Windows.Foundation.Collections.IMapView<System.String,System.Object>;
            // into
            //      Windows.Foundation.Collections.IMapView<string,any>;
            // 
            typeMap.Each(item =>
            {
                fromName = fromName.Replace(item.Key, item.Value);
            });

            // If it's an array type return it as such.
            var genericTypeArgName = "";
            if (IsTypeArray(Type, out genericTypeArgName))
            {
                return genericTypeArgName + "[]";
            }

            // remove the generic bit
            return fromName;
        }

        private static bool IsTypeArray(Type Type, out string genericTypeArgName)
        {
            genericTypeArgName = "";

            return Type.IsConstructedGenericType && IsTypeTypeArray(Type, out genericTypeArgName);
        }
        private static bool IsTypeTypeArray(Type Type, out string genericTypeArgName)
        {
            var genericTypeInstanceReference = Type;// as GenericInstanceType;
            if (genericTypeInstanceReference != null)
            {
                genericTypeArgName = genericTypeInstanceReference == null
                    ? "T"
                    : genericTypeInstanceReference.GenericTypeArguments[0].ToTypeScriptType();

                var enumerableNamePrefix = "System.Collections.IEnumerable";

                // is this IEnumerable?
                if (Type.FullName.StartsWith(enumerableNamePrefix))
                {
                    return true;
                }

                // does it have an interface that implements IEnumerable?
                var possibleListType = GetTypeDefinition(genericTypeInstanceReference.GetElementType());
                if (possibleListType != null)
                {
                    if (possibleListType.GetInterfaces().Any(x => x.FullName.StartsWith(enumerableNamePrefix)))
                    {
                        return true;
                    }
                }

                // TODO: do we need to work harder at inspecting interface items?
                // TODO: write tests to prove it..
            }



            genericTypeArgName = "";
            return false;
        }


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



        //TODO: move this somewhere else?
        public static string ToTypeScriptTypeName(this Type typeReference)
        {
            if (genericTypeMap == null)
            {
                genericTypeMap = typeMap
                    .ToDictionary(item => "<" + item.Key + ">", item => "<" + item.Value + ">");

                typeMap
                    .ToDictionary(item => item.Key + "[]", item => item.Value + "[]")
                    .Each(x => genericTypeMap.Add(x.Key, x.Value));
            }

            //TODO: hacky
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

            // translate / in nested classes into underscores
            fromName = fromName.Replace("/", "_");

            if (typeMap.ContainsKey(fromName))
            {
                return typeMap[fromName];
            }

            var genericType = genericTypeMap.FirstOrDefault(x => fromName.Contains(x.Key));
            if (!genericType.Equals(default(KeyValuePair<string, string>)))
            {
                fromName = fromName.Replace(genericType.Key, genericType.Value);
            }

            fromName = fromName
                .StripGenericTick()
                .StripOutParamSymbol();

            // To lazy to figure out the Mono.Cecil way (or if there is a way), but do 
            // some string search/replace on types for example:
            //
            // turn
            //      Windows.Foundation.Collections.IMapView<System.String,System.Object>;
            // into
            //      Windows.Foundation.Collections.IMapView<string,any>;
            // 
            typeMap.Each(item =>
            {
                fromName = fromName.Replace(item.Key, item.Value);
            });

            // remove the generic bit
            return fromName;
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

        public static string ToTypeScript(this Type type, TsdConfig config = null)
        {
            if (config == null)
                config = new TsdConfig();

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
