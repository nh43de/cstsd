namespace ToTypeScriptD.Core.Ts
{
    public class TsField : TsMember
    {
        public TsType FieldType { get; set; }

        public bool IsNullable { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}