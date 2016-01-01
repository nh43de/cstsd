namespace ToTypeScriptD.Core.TypeScript
{
    public class TSInterfaceMember : TSField
    {
        public bool IsOptional { get; set; }

        public override string ToString()
        {
            var optStr = IsOptional ? "?" : "";
            return $"{Name}{optStr} : {Type}";
        }
     
    }
}