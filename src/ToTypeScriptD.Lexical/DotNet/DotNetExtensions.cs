using System;
using System.Collections.Generic;
using System.Linq;
using ToTypeScriptD.Lexical.Extensions;
using ToTypeScriptD.Lexical.WinMD;

namespace ToTypeScriptD.Lexical.DotNet
{

    public static class DotNetExtensions
    {

        static Dictionary<string, string> typeMap = new Dictionary<string, string>{
                { "System.String",               "string"},
                { "System.Int16",                "number"},
                { "System.Int32",                "number"},
                { "System.Int64",                "number"},
                { "System.UInt16",               "number"},
                { "System.UInt32",               "number"},
                { "System.UInt64",               "number"},
                { "System.Object",               "any"},
                { "System.DateTime",             "Date"},
                { "System.Void",                 "void"},
                { "System.Boolean",              "boolean"},
                { "System.IntPtr",               "number"},
                { "System.Byte",                 "number"}, // TODO: Confirm if this is the correct representation?
                { "System.Single",               "number"},
                { "System.Double",               "number"},
                { "System.Char",                 "number"}, // TODO: should this be a string or number?
                { "System.Guid",                 "string"},
                { "System.Byte[]",               "string"},
        };
        static Dictionary<string, string> genericTypeMap = null;

        public static bool ShouldIgnoreType(this Type name)
        {
            if (!name.IsPublic)
                return true;

            return false;
        }

        public static bool ShouldIgnoreTypeByName(this Type Type)
        {
            if (Type.FullName == "System.Collections.IEnumerable")
                return true;

            return false;
        }

        private static bool IsNullable(this Type Type)
        {
            if (Type.IsValueType)
                return false;

            // TODO: is there a better way to determine if it's a Nullable?
            return Type.Namespace == "System" && Type.Name == "Nullable`1";
        }

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
    }
}
