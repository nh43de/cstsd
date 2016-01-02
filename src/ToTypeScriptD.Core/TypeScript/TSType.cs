namespace ToTypeScriptD.Core.TypeScript
{
    public class TSType
    {
        public TSType(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}