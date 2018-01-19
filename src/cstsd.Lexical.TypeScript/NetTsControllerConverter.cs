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

            var ajaxMethods = controllerMethods
                .Where(m => m.ReturnType.Name != "IActionResult")
                .Where(m => m.IsPublic && m.Attributes.Any(a => a == "TsExport"))
                .Select(a => GetControllerExecFunction(a, controllerNetClass.Name))
                .ToArray();


            var httpMethods = controllerMethods
                .Where(m => m.ReturnType.Name == "IActionResult")
                .Where(m => m.IsPublic && m.Attributes.Any(a => a == "TsExport"))
                .Select(a => GetControllerNavigateFunction(a, controllerNetClass.Name))
                .ToArray();

            return new TsModule
            {
                Name = controllerNetClass.Name,
                FunctionDeclarations = httpMethods.Concat(ajaxMethods).ToArray(),
                FieldDeclarations = GetFields(controllerMethods, controllerNetClass),
                IsExport = true
            };
        }

        public static string GetControllerName(string controllerName)
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

            var dataParametersString = GetDataParametersString(a);

            //last function parameter is always a callback
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

        public static string GetDataParametersString(TsFunction tsFunction)
        {
            var dataParametersString = string.Join(",\r\n", tsFunction.Parameters.Select(p => $"{p.Name}: {p.Name}"));

            return dataParametersString;
        }

        public TsFunction GetControllerNavigateFunction(NetMethod netMethod, string controllerName)
        {
            var tsFunction = GetTsFunction(netMethod);

            tsFunction.ReturnType = new TsType
            {
                Name = "void"
            };

            var actionName = netMethod.Name;
            var dataParametersString = GetDataParametersString(tsFunction);

            tsFunction.FunctionBody =
@"return frameworkNavigate({
	route: " + $"this.{actionName}Route" + @",
	data: {
" + dataParametersString.Indent("\t\t") + @"
	}
});";
            return tsFunction;
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
            var routeStr = GetRouteDataFieldDeclarationString(netMethod, controllerNetClass);

            var a = new TsFieldDeclaration
            {
                DefaultValue = routeStr,
                FieldDeclarationType = FieldDeclarationType.Const,
                FieldType = new TsType { Name = "IRouteData" },
                IsStatic = true,
                Name = netMethod.Name + "Route"
            };
            return a;
        }

        public static string GetRouteDataFieldDeclarationString(NetMethod netMethod, NetClass controllerNetClass)
        {
            var route = GetRouteInfo(controllerNetClass, netMethod);

            var r =
$@"{{
    baseUrl: '/',
    controller: '{route.Controller}',
    action: '{route.Action}'
}}";

            return r;
        }


        public static RouteInfo GetRouteInfo(NetClass controllerNetClass, NetMethod netMethod)
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