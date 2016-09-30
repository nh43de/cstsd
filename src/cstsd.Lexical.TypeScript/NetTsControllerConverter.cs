using System.Linq;
using cstsd.Core.Extensions;
using cstsd.Core.Net;
using cstsd.Core.Ts;

namespace cstsd.TypeScript
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
                    .Where(m => m.IsPublic && m.Attributes.Any(a => a == "TsExport"))
                    .Select(a => GetTsFunction(a, controllerNetClass))
                    .ToList(),
                IsExport = true
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
        

            var functionReturnType = GetTsTypeName(netMethod.ReturnType, true);

            a.ReturnType = new TsType
            {
                Name = "void"
            };

            
            var controllerName = GetControllerName(controllerNetClass.Name);
            var actionName = netMethod.Name;
            
            //a.FunctionBody = $"/* controller: {controllerNetClass.Name}; action: {netMethod.Name} */";

            var dataParametersString = string.Join(",\r\n",  a.Parameters.Select(p => $"{p.Name}: {p.Name}"));

            a.Parameters.Add(new TsParameter
            {
                FieldType = new TsType
                {
                    Name = $"(response: {functionReturnType}) => void"
                },
                Name = "callback"
            });

            a.FunctionBody =
@"$.ajax({
	url: """ + $"/{controllerName}/{actionName}" + @""",
	data: {
" + dataParametersString.Indent("\t\t\t") + @"
	},
	type: ""GET"",
	//data: idRequestData,
	dataType: ""JSON"",
	success(response: " + functionReturnType + @") {
		$(""#debugOut"").text(JSON.stringify(response));
		callback(response);
	},
	error(response) {
		var a: " + functionReturnType + @" = {
			responseObject: null,
			message: ""XHR Error"",
			responseCode: ServiceResponseCode.Failed
		};
		callback(a);
	}
});";

            return a;
        }
    }
}