using System;
using System.Linq;
using System.Text.RegularExpressions;
using cstsd.Core.Extensions;
using cstsd.Core.Net;
using cstsd.Core.Ts;
using System.Collections.Generic;


namespace cstsd.TypeScript
{
    public class NetTsControllerConverter : NetTsConverter
    {
        public virtual TsModule GetControllerTsModule(NetClass controllerNetClass)
        {
            var controllerMethods = controllerNetClass
                .Methods
                .ToList();

            return new TsModule
            {
                Name = controllerNetClass.Name,
                FunctionDeclarations = controllerMethods
                    .Where(m => m.ReturnType.Name != "IActionResult")
                    .Where(m => m.IsPublic && m.Attributes.Any(a => a == "TsExport"))
                    .Select(a => GetControllerExecFunction(a, controllerNetClass.Name))
                    .ToList(),
                FieldDeclarations = GetFields(controllerMethods, controllerNetClass),
                IsExport = true
            };
        }

        public string GetControllerName(string controllerName)
        {
            if (controllerName.EndsWith("Controller"))
                return controllerName.Substring(0, controllerName.Length - "Controller".Length);

            return controllerName;
        }

        public List<TsFieldDeclaration> GetFields(List<NetMethod> controllerMethods, NetClass controllerNetClass)
        {
            var distinctControllerMethods = controllerMethods.Select(m => m.Name)
                .Distinct()
                .GroupJoin(
                    controllerMethods,
                    name => name,
                    netMethod => netMethod.Name,
                    (name, netMethods) => netMethods.First());

            var urlStringFields =
                   distinctControllerMethods.Select(m => GetUrlNavigateConstFieldDeclaration(m, controllerNetClass));
            
            var routeDataFields =
                   distinctControllerMethods.Select(m => GetRouteDataFieldDeclaration(m, controllerNetClass));

            return urlStringFields.Union(routeDataFields).ToList();
        }


        public TsFunction GetControllerExecFunction(NetMethod netMethod, string controllerName)
        {
            var a = base.GetTsFunction(netMethod);

            var functionReturnType = GetTsType(netMethod.ReturnType).Name;

            a.ReturnType = new TsType
            {
                Name = "JQueryXHR"
            };

            var actionType = netMethod.Attributes.Any(attr => string.Equals(attr, "HttpGet", StringComparison.InvariantCultureIgnoreCase)) ? "GET" : "POST";
            actionType = netMethod.Attributes.Any(attr => string.Equals(attr, "HttpPost", StringComparison.InvariantCultureIgnoreCase)) ? "POST" : actionType;

            //a.FunctionBody = $"/* controller: {controllerNetClass.Name}; action: {netMethod.Name} */";

            var dataParametersString = string.Join(",\r\n", a.Parameters.Select(p => $"{p.Name}: {p.Name}"));

            a.Parameters.Add(new TsParameter
            {
                FieldType = new TsType
                {
                    Name = $"(response: {functionReturnType}) => void"
                },
                Name = "callback"
            });

            a.FunctionBody =
@"return frameworkExec({
	url: " + $"{controllerName}.{netMethod.Name}Url" + @",
	data: {
" + dataParametersString.Indent("\t\t") + @"
	},
	type: """ + actionType + @""",
	callback: callback
});";

            return a;
        }

        public TsFieldDeclaration GetUrlNavigateConstFieldDeclaration(NetMethod netMethod, NetClass controllerNetClass)
        {
            var route = GetRouteInfo(controllerNetClass, netMethod);

            var a = new TsFieldDeclaration
            {
                DefaultValue = "\"" + route.Url + "\"",
                FieldDeclarationType = FieldDeclarationType.Const,
                FieldType = new TsType { Name = "string" },
                IsStatic = true,
                Name = netMethod.Name + "Url"
            };
            return a;
        }

        public TsFieldDeclaration GetRouteDataFieldDeclaration(NetMethod netMethod, NetClass controllerNetClass)
        {
            var route = GetRouteInfo(controllerNetClass, netMethod);

            var a = new TsFieldDeclaration
            {
                DefaultValue = 
$@"{{
    baseUrl: '/',
    controller: '{route.Controller}',
    action: '{route.Action}'
}}",
                FieldDeclarationType = FieldDeclarationType.Const,
                FieldType = new TsType { Name = "IRouteData" },
                IsStatic = true,
                Name = netMethod.Name + "Route"
            };
            return a;
        }
        
        public RouteInfo GetRouteInfo(NetClass controllerNetClass, NetMethod netMethod)
        {
            var controllerName = GetControllerName(controllerNetClass.Name);
            var actionName = netMethod.Name;
            var routeInfo = controllerNetClass.Attributes.FirstOrDefault(attr => attr.StartsWith("Route"));

            if (string.IsNullOrWhiteSpace(routeInfo))
            {
                routeInfo = "[controller]/[action]";
            }
            else
            {
                var m = Regex.Match(routeInfo, "\"(.*?)\"");

                if (m.Success && m.Groups.Count > 1 && m.Groups[1].Success)
                {
                    routeInfo = m.Groups[1].Value;
                }
            }

            var route = routeInfo
                .Replace("[controller]", controllerName)
                .Replace("[action]", actionName);

            if (!route.StartsWith("/"))
                route = "/" + route;

            return new RouteInfo
            {
                Url = route,
                Controller = controllerName,
                Action = actionName
            };
        }

        public class RouteInfo
        {
            public string Controller { get; set; }
            public string Action { get; set; }

            public string Url { get; set; }
        }


    }
}