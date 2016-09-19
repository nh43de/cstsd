using System.Linq;
using ToTypeScriptD.Core.Net;
using ToTypeScriptD.Core.Ts;

namespace cstsd.Lexical.TypeScript
{
    public class NetTsControllerConverter : NetTsConverter
    {
        public virtual TsModule GetControllerTsModule(NetClass controllerNetClass)
        {
            return new TsModule
            {
                Name = controllerNetClass.Name,
                FunctionDeclarations = controllerNetClass
                    .Methods
                    .Where(m => m.IsPublic)
                    .Select(a => GetTsFunction(a, controllerNetClass))
                    .ToList()
            };
        }

        

        public TsFunction GetTsFunction(NetMethod netMethod, NetClass controllerNetClass)
        {
            var a = base.GetTsFunction(netMethod);

            a.FunctionBody = $"/* controller: {controllerNetClass.Name}; action: {netMethod.Name} */";

            return a;
        }
    }
}