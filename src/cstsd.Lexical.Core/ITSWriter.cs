using ToTypeScriptD.Lexical.TypeScript;
using ToTypeScriptD.Lexical.TypeScript.Abstract;

namespace ToTypeScriptD.Lexical
{
    public interface ITSWriter
    {
        string Write(TSModule tsModule);
        string Write(TSModuleTypeDeclaration tsModuleTypeDeclaration);
        string Write(TSEnum tsEnum);
        string Write(TSGenericParameter tsGenericParameter);
        string Write(TSField tsField);
        string Write(TSMethod tsMethod);
        string Write(TSGenericType tsGenericType);
        string Write(TSType tsType);
        string Write(TSInterface tsInterface);
        string Write(TSEvent tsEvent);
        string Write(TSClass tsClass);
    }
}