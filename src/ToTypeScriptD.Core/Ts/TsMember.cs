namespace ToTypeScriptD.Core.Ts
{
    public class TsMember
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; } = false;
    }
}