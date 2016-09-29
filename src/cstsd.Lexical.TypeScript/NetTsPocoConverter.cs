using System.Linq;
using ToTypeScriptD.Core.Net;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class NetTsPocoConverter : NetTsConverter
    {
        public TsInterface GetTsInterface(NetClass netClass)
        {
            return new TsInterface
            {
                IsPublic = netClass.IsPublic,
                Name = netClass.Name,
                Fields = netClass
                    .Properties
                    .Where(p => !p.Attributes.Contains("TsExcludeAttribute"))
                    .Select(GetTsField)
                    .ToList()


                //Fields = 
            };
        }
    }
}