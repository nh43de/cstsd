using System;
using System.Collections.Generic;
using System.Linq;
using cstsd.Lexical.TypeScript.Extensions;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Core.Net;

namespace cstsd.Lexical.TypeScript
{
    public static class TypeMappings
    {
        public static readonly Dictionary<string, string> Default = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase){
                { "System.Boolean",              "boolean"},
                { "System.Byte",                 "any /* byte */"},
                { "System.Byte[]",               "any /* byte[] */"},
                { "System.Char",                 "string /* char */"},
                { "System.Char[]",               "string"},
                { "System.DateTime",             "Date"},
                { "System.Decimal",              "number"},
                { "System.Double",               "number"},
                { "System.Guid",                 "string /* guid */"},
                { "System.Int16",                "number /* Int16 */"},
                { "System.Int32",                "number"},
                { "System.Int64",                "number"},
                { "System.IntPtr",               "number /* IntPtr */"},
                { "System.Object",               "any"},
                { "System.Single",               "number"},
                { "System.String",               "string"},
                { "System.Type",                 "string /* System.Type? */"},
                { "System.UInt16",               "number /* UInt16 */"},
                { "System.UInt32",               "number"},
                { "System.UInt64",               "number"},
                { "System.Void",                 "void"},
                { "Boolean",              "boolean"},
                { "Bool",                 "boolean"},
                { "Int",                  "number"},
                { "Byte",                 "any /* byte */"},
                { "Byte[]",               "any /* byte[] */"},
                { "Char",                 "string /* char */"},
                { "Char[]",               "string"},
                { "DateTime",             "Date"},
                { "Decimal",              "number"},
                { "Double",               "number"},
                { "Guid",                 "string /* guid */"},
                { "Int16",                "number /* Int16 */"},
                { "Int32",                "number"},
                { "Int64",                "number"},
                { "IntPtr",               "number /* IntPtr */"},
                { "Object",               "any"},
                { "Single",               "number"},
                { "String",               "string"},
                { "Type",                 "string /* Type? */"},
                { "UInt16",               "number /* UInt16 */"},
                { "UInt32",               "number"},
                { "UInt64",               "number"},
                { "Void",                 "void"}
        };
    }

    public static class TypeHelperExtensions
    {

        static readonly Lazy<Dictionary<string, string>> GenericTypeMap = new Lazy<Dictionary<string, string>>(() =>
        {
            var a = TypeMappings.Default
                .ToDictionary(item => "<" + item.Key + ">", item => "<" + item.Value + ">");

            TypeMappings.Default
                .ToDictionary(item => item.Key + "[]", item => item.Value + "[]")
                    .Each(x => a.Add(x.Key, x.Value));

            return a;
        });

        //TODO: what?
        public static string ToTypeScriptItemName(this Type type)
        {
            // Nested classes don't report their namespace. So we have to walk up the 
            // DeclaringType tree to find the root most type to grab it's namespace.
            var parentMostType = type;
            while (parentMostType.DeclaringType != null)
            {
                parentMostType = parentMostType.DeclaringType;
            }

            var mainTypeName = type.FullName;

            // trim namespace off of the front.
            mainTypeName = mainTypeName.Substring(parentMostType.Namespace.Length + 1);

            // replace the nested class slash with an underscore
            mainTypeName = mainTypeName.Replace("/", "_").StripGenericTick();

            return mainTypeName.StripGenericTick();
        }

        public static string GetArrayname(this Type typeReference)
        {
            string fromName;

            Func<Type, bool> collectionInterfaces =
                i =>
                    (i.Name.Contains("ICollection") || i.Name.Contains("IEnumerable"));

            if (typeReference.GetInterfaces().Any(collectionInterfaces))
            {
                var iCollectionTypes = typeReference.GetInterfaces().Where(collectionInterfaces);
                var iCollectionGenertc = iCollectionTypes.FirstOrDefault(i => i.GetGenericArguments().Any()); //TODO: repeat getintfcs() - fix
                fromName = iCollectionGenertc == default(Type) ? $"System.Int32[]" : $"{iCollectionGenertc.GetGenericArguments()[0].FullName ?? iCollectionGenertc.GetGenericArguments()[0].Name}[]";
            }
            else
            {
                fromName = String.IsNullOrWhiteSpace(typeReference.FullName) ? typeReference.Name : typeReference.FullName;
            }

            return fromName;
        }

        public static string ToTypeScriptTypeName(this NetType netType)
        {
            var fromName = netType.Name;

            if (TypeMappings.Default.ContainsKey(fromName))
            {
                return TypeMappings.Default[fromName];
            }

            if (!netType.GenericParameters.Any()) return fromName;
            
            var genericParamsStr = string.Join(",", netType.GenericParameters.Select(gp => gp.Name));
                
            return $"{fromName}<{genericParamsStr}>";
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
            if (TypeMappings.Default.ContainsKey(fromName))
            {
                return TypeMappings.Default[fromName];
            }

            // otherwise check for generic type mapping
            var genericType = GenericTypeMap.Value.FirstOrDefault(x => fromName.Contains(x.Key));
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

            TypeMappings.Default.Each(item =>
            {
                fromName = fromName.Replace(item.Key, item.Value);
            });

            return fromName;
        }

        
        //TODO: What does this do?
        public static Type GetTypeDefinition(Type type)
        {
            if (type == null)
                return null;

            try
            {
                var ass = type.Assembly;

                var result = ass.Modules
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(td => td.FullName == type.FullName);

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

        //TODO: move this somewhere else?
        public static bool IsNullable(this Type type)
        {
            var typeName = type.Name;
            //var typeFullName = Type.FullName;
            
            return (type.Namespace == "System" && typeName == "Nullable`1") || type.IsValueType == false;
        }

        public static Type GetNullableType(this Type type)
        {
            if (!(type.Namespace == "System" && type.Name == "Nullable`1")) return type;

            var genericInstanceType = type;// as GenericInstanceType;
            type = genericInstanceType.GetGenericArguments()[0];

            return type;
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


        // TODO: look to move this to the WinMDExtensions.cs
        public static string ToTypeScriptItemNameWinMd(this Type typeReference)
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
    }
}