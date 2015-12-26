
namespace ToTypeScriptD.Core.DotNet
{
    public class DotNetConfig : ConfigBase
    {

        public override bool CamelBackCase { get; set; } = true;

        public override TypeWriters.ITypeWriterTypeSelector GetTypeWriterTypeSelector()
        {
            return new DotNet.DotNetTypeWriterTypeSelector();
        }
    }
}
