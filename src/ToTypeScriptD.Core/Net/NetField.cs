namespace ToTypeScriptD.Core.Net
{
    public class NetField : NetMember
    {
        public NetType FieldType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}