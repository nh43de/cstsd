using ToTypeScriptD.Core;

namespace cstsd.Lexical.Core
{
    public interface ITSWriter
    {
        string Write(NetModule netModule);
        string Write(TSModuleTypeDeclaration tsModuleTypeDeclaration);
        string Write(NetEnum netEnum);
        string Write(NetGenericParameter netGenericParameter);
        string Write(NetField netField);
        string Write(NetMethod netMethod);
        string Write(NetGenericType netGenericType);
        string Write(NetType netType);
        string Write(NetInterface netInterface);
        string Write(NetEvent netEvent);
        string Write(NetClass netClass);
    }
}