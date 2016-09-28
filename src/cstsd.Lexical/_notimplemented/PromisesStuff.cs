//TODO: implement promise stuff

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using ToTypeScriptD.Core.Config;
//using ToTypeScriptD.Core.Extensions;
//using ToTypeScriptD.Core.TypeScript;
//using ToTypeScriptD.Lexical.DotNet;
//using ToTypeScriptD.Lexical.Extensions;

//namespace ToTypeScriptD.Lexical.WinMD
//{
//    public class PromisesStuff
//    {
//        //TODO: these need to be moved to the writing part
//        public static readonly ConfigBase Config;


//        protected static int IndentCount;

//        public static void Indent(StringBuilder sb) => sb.Append(IndentValue);

//        public static string IndentValue => Config.Indent.Dup(IndentCount);
        


//        #region Promise Extension

//        private static void WriteAsyncPromiseMethods(StringBuilder sb, Type td)
//        {
//            string genericTypeArgName;
//            if (IsTypeAsync(out genericTypeArgName, td))
//            {
//                sb.AppendLine();
//                Indent(sb); Indent(sb); sb.AppendFormatLine("// Promise Extension");
//                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => ToTypeScriptD.WinRT.IPromise<U>, error?: (error: any) => ToTypeScriptD.WinRT.IPromise<U>, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => ToTypeScriptD.WinRT.IPromise<U>, error?: (error: any) => U, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => U, error?: (error: any) => ToTypeScriptD.WinRT.IPromise<U>, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("then<U>(success?: (value: {0}) => U, error?: (error: any) => U, progress?: (progress: any) => void): ToTypeScriptD.WinRT.IPromise<U>;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("done<U>(success?: (value: {0}) => any, error?: (error: any) => any, progress?: (progress: any) => void): void;", genericTypeArgName);
//            }
//        }

//        private static bool IsTypeAsync(out string genericTypeArgName, Type td)
//        {
//            var currType = td;

//            if (IsTypeAsync(td, out genericTypeArgName))
//            {
//                return true;
//            }

//            foreach (var i in td.GetInterfaces())
//            {
//                if (IsTypeAsync(i, out genericTypeArgName))
//                {
//                    return true;
//                }
//            }
//            genericTypeArgName = "";
//            return false;
//        }

//        private static bool IsTypeAsync(Type typeReference, out string genericTypeArgName)
//        {
//            if (typeReference.FullName.StartsWith("Windows.Foundation.IAsyncOperation`1") ||
//                typeReference.FullName.StartsWith("Windows.Foundation.IAsyncOperationWithProgress`2")
//                )
//            {
//                var genericInstanceType = typeReference;// as GenericInstanceType;
//                if (genericInstanceType == null)
//                {
//                    genericTypeArgName = "TResult";
//                }
//                else
//                {
//                    genericTypeArgName = genericInstanceType.GetGenericArguments()[0].ToTypeScriptTypeName();
//                }
//                return true;
//            }

//            genericTypeArgName = "";
//            return false;
//        }
        
//        #endregion

//        #region Array Extension
//        private static void WriteVectorArrayPrototypeExtensions(StringBuilder sb, bool wroteALengthProperty, Type td)
//        {
//            string genericTypeArgName;
//            if (IsTypeArray(out genericTypeArgName, td))
//            {
//                sb.AppendLine();
//                Indent(sb); Indent(sb); sb.AppendFormatLine("// Array.prototype extensions", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("toString(): string;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("toLocaleString(): string;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("concat(...items: {0}[][]): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("join(seperator: string): string;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("pop(): {0};", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("push(...items: {0}[]): void;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("reverse(): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("shift(): {0};", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("slice(start: number): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("slice(start: number, end: number): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("sort(): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("sort(compareFn: (a: {0}, b: {0}) => number): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("splice(start: number): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("splice(start: number, deleteCount: number, ...items: {0}[]): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("unshift(...items: {0}[]): number;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("lastIndexOf(searchElement: {0}): number;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("lastIndexOf(searchElement: {0}, fromIndex: number): number;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("every(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): boolean;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("every(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): boolean;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("some(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): boolean;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("some(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): boolean;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("forEach(callbackfn: (value: {0}, index: number, array: {0}[]) => void ): void;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("forEach(callbackfn: (value: {0}, index: number, array: {0}[]) => void , thisArg: any): void;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("map(callbackfn: (value: {0}, index: number, array: {0}[]) => any): any[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("map(callbackfn: (value: {0}, index: number, array: {0}[]) => any, thisArg: any): any[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("filter(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("filter(callbackfn: (value: {0}, index: number, array: {0}[]) => boolean, thisArg: any): {0}[];", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("reduce(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any): any;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("reduce(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any, initialValue: any): any;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("reduceRight(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any): any;", genericTypeArgName);
//                Indent(sb); Indent(sb); sb.AppendFormatLine("reduceRight(callbackfn: (previousValue: any, currentValue: any, currentIndex: number, array: {0}[]) => any, initialValue: any): any;", genericTypeArgName);
//                if (!wroteALengthProperty)
//                {
//                    Indent(sb); Indent(sb); sb.AppendFormatLine("length: number;", genericTypeArgName);
//                }

//            }
//        }

//        private static bool IsTypeArray(out string genericTypeArgName, Type td)
//        {
//            var currType = td;

//            if (IsTypeArray(td, out genericTypeArgName))
//            {
//                return true;
//            }

//            foreach (var i in td.GetInterfaces())
//            {
//                if (IsTypeArray(i, out genericTypeArgName))
//                {
//                    return true;
//                }
//            }
//            genericTypeArgName = "";
//            return false;
//        }

//        private static bool IsTypeArray(Type typeReference, out string genericTypeArgName)
//        {
//            if (typeReference.FullName.StartsWith("Windows.Foundation.Collections.IVector`1") ||
//                typeReference.FullName.StartsWith("Windows.Foundation.Collections.IVectorView`1")
//                )
//            {
//                var genericInstanceType = typeReference; //as GenericInstanceType;
//                if (genericInstanceType == null)
//                {
//                    genericTypeArgName = "T";
//                }
//                else
//                {
//                    genericTypeArgName = genericInstanceType.GetGenericArguments()[0].ToTypeScriptTypeName();
//                }
//                return true;
//            }

//            genericTypeArgName = "";
//            return false;
//        }

//        #endregion


//    }
    
//}
