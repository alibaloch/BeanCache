using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(BeanCacheWeb.Startup))]

namespace BeanCacheWeb
{
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;
    using System.Web.Http;
    public class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            FormatterConfig.ConfigureFormatters(config.Formatters);
            RouteConfig.RegisterRoutes(config.Routes);
            appBuilder.UseCors(CorsOptions.AllowAll);

            SetUpStaticFileHosting(appBuilder);
            appBuilder.UseWebApi(config);
        }

        private void SetUpStaticFileHosting(IAppBuilder appBuilder)
        {
            var options = new FileServerOptions
            {
                RequestPath = new PathString("/app"),
                EnableDirectoryBrowsing = true
            };
            appBuilder.UseFileServer(options);
            appBuilder.UseStaticFiles();
        }
    }
}
