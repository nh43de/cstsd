namespace cstsd.Core.Net
{
    public enum NetFieldDeclarationType
    {
        Const,
        Var,
        ReadOnly
    }

    public class NetFieldDeclaration : NetField
    {
        public NetFieldDeclarationType FieldDeclarationType { get; set; }
    }


    public class NetField : NetMember
    {
        public string DefaultValue { get; set; }

        public NetType FieldType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}