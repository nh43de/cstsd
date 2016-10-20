namespace cstsd.Core.Ts
{
    public enum FieldDeclarationType
    {
        Const,
        Var,
        Let
    }

    public class TsFieldDeclaration : TsField
    {
        public FieldDeclarationType FieldDeclarationType { get; set; }
    }

    public class TsField : TsMember
    {
        public TsType FieldType { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}