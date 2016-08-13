using ToTypeScriptD.Core;

namespace cstsd.Lexical.Core
{
    public interface INetWriter
    {
        string Write(NetNamespace netNamespace);
        string Write(NetEnum netEnum);
        string Write(NetGenericParameter netGenericParameter);
        string Write(NetField netField);
        string Write(NetMethod netMethod);
        string Write(NetType netType);
        string Write(NetInterface netInterface);
        string Write(NetEvent netEvent);
        string Write(NetClass netClass);
    }
}