using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanCacheWeb
{
    using System.Web.Http;

    public static class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.IgnoreRoute("Html", "{whatever}.html/{*pathInfo}");
            routes.IgnoreRoute("FilesRoute", "app/{*pathInfo}");

            routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "{controller}/{action}/{id}",
                    defaults: new { controller = "Default",
                        action = "Get", id = RouteParameter.Optional }
                );
        }
    }
}
