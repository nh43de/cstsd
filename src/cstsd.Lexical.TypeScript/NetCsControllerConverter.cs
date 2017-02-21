using System;
using System.Linq;
using System.Text.RegularExpressions;
using cstsd.Core.Extensions;
using cstsd.Core.Net;
using cstsd.Core.Ts;
using System.Collections.Generic;


namespace cstsd.TypeScript
{
    //TODO: rename to API generator?
    public class NetCsControllerConverter
    {
        public virtual NetClass GetControllerApiClientCsClass(NetClass controllerNetClass)
        {
            var controllerMethods = controllerNetClass
                .Methods
                .ToList();
            
            return new NetClass
            {
                Name = controllerNetClass.Name,
                Methods = controllerMethods
                    .Where(m => m.ReturnType.Name != "IActionResult")
                    .Where(m => m.IsPublic && m.Attributes.Any(a => a == "CsExport"))
                    .Select(a => GetControllerExecFunction(a, controllerNetClass.Name))
                    .ToList(),
                Fields =  GetFields(controllerMethods, controllerNetClass),
                IsPublic = true
            };
        }

        private string GetControllerName(string controllerName)
        {
            if (controllerName.EndsWith("Controller"))
                return controllerName.Substring(0, controllerName.Length - "Controller".Length);

            return controllerName;
        }

        private List<NetFieldDeclaration> GetFields(List<NetMethod> controllerMethods, NetClass controllerNetClass)
        {
            var distinctControllerMethods = controllerMethods.Select(m => m.Name)
                .Distinct()
                .GroupJoin(
                    controllerMethods,
                    name => name,
                    netMethod => netMethod.Name,
                    (name, netMethods) => netMethods.First())
                .ToArray();

            //these are just the url fields
            var urlStringFields =
                   distinctControllerMethods.Select(m => GetUrlNavigateConstFieldDeclaration(m, controllerNetClass));
 
            //these are the route data objects           
            //var routeDataFields =
            //       distinctControllerMethods.Select(m => GetRouteDataFieldDeclaration(m, controllerNetClass));

            return urlStringFields.ToList(); //.Union(routeDataFields).ToList();
        }

        private NetMethod GetControllerExecFunction(NetMethod netMethod, string controllerName)
        {
            var functionReturnType = netMethod.ReturnType;

            netMethod.ReturnType = new NetType
            {
                Name = "RestRequestAsyncHandle"
            };

            //default to Post
            var actionType = netMethod.Attributes.Any(attr => string.Equals(attr, "HttpGet", StringComparison.InvariantCultureIgnoreCase)) ? "GET" : "POST";
            actionType = netMethod.Attributes.Any(attr => string.Equals(attr, "HttpPost", StringComparison.InvariantCultureIgnoreCase)) ? "POST" : actionType;
            
            var dataParametersString = string.Join(",\r\n", netMethod.Parameters.Select(p => $"{p.Name}"));

            netMethod.Parameters.Add(new NetParameter
            {
                FieldType = new NetType
                {
                    Name = $"Action<IRestResponse<{functionReturnType}>>"
                },
                Name = "callback"
            });

            netMethod.MethodBody =
@"return ServiceFramework.FrameworkExec(
    baseUrl: _baseUrl,
	url: " + $"{netMethod.Name}Url" + @",
	data: new {
" + dataParametersString.Indent("\t\t") + @"
	},
	type: """ + actionType + @""",
	callback: callback
);";

            return netMethod;
        }

        private NetFieldDeclaration GetUrlNavigateConstFieldDeclaration(NetMethod netMethod, NetClass controllerNetClass)
        {
            var route = GetRouteInfo(controllerNetClass, netMethod);

            var a = new NetFieldDeclaration
            {
                DefaultValue = "\"" + route.Url + "\"",
                FieldDeclarationType = NetFieldDeclarationType.Const,
                FieldType = new NetType { Name = "string" },
                IsPublic = true,
                Name = netMethod.Name + "Url"
            };
            return a;
        }

        //TODO: no route info classes for now
        //        public NetFieldDeclaration GetRouteDataFieldDeclaration(NetMethod netMethod, NetClass controllerNetClass)
        //        {
        //            var route = GetRouteInfo(controllerNetClass, netMethod);

        //            var a = new NetFieldDeclaration
        //            {
        //                DefaultValue = 
        //$@"{{
        //    baseUrl: '/',
        //    controller: '{route.Controller}',
        //    action: '{route.Action}'
        //}}",
        //                FieldDeclarationType = NetFieldDeclarationType.Const,
        //                FieldType = new NetType { Name = "IRouteData" },
        //                IsStatic = true,
        //                Name = netMethod.Name + "Route"
        //            };
        //            return a;
        //        }

        private RouteInfo GetRouteInfo(NetClass controllerNetClass, NetMethod netMethod)
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