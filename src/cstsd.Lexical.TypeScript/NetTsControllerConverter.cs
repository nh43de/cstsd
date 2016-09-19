using System.CodeDom;
using System.Linq;
using ToTypeScriptD.Core.Extensions;
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

        public string GetControllerName(string controllerName)
        {
            if (controllerName.EndsWith("Controller"))
                return controllerName.Substring(0, controllerName.Length - "Controller".Length);

            return controllerName;
        }

        public TsFunction GetTsFunction(NetMethod netMethod, NetClass controllerNetClass)
        {
            var a = base.GetTsFunction(netMethod);

            var controllerName = GetControllerName(controllerNetClass.Name);
            var actionName = netMethod.Name;
            
            //a.FunctionBody = $"/* controller: {controllerNetClass.Name}; action: {netMethod.Name} */";

            var dataParameters = string.Join(",\r\n",  a.Parameters.Select(p => $"{p.Name}: {p.FieldType.Name}"));
            
            a.FunctionBody = @"
$.ajax({
	url: """ + $"/api/{controllerName}/{actionName}" + @""",
	data: {
" +         dataParameters.Indent("\t\t\t") + @"
	},
	type: ""POST"",
	//data: idRequestData,
	dataType: ""JSON"",
	success(response: " + GetTsTypeName(netMethod.ReturnType) + @") {
		$(""#debugOut"").text(JSON.stringify(response));
		callback(response);
	},
	error(response) {
		var a: " + GetTsTypeName(netMethod.ReturnType) + @" = {
			responseObject: null,
			message: ""XHR Error"",
			responseCode: ServiceResponseCode.Failed
		};
		callback(a);
	}
});
";

            return a;
        }
    }
}