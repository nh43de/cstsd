namespace ToTypeScriptD.Core.TypeScript
{
    public class TSField
    {
        public string Name { get; set; }
        public TSType Type { get; set; }

        public override string ToString()
        {
            return $"{Name} : {Type}";
        }
    }
}