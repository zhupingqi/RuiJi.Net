using Microsoft.Owin.Hosting;
using Microsoft.Owin.FileSystems;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin;
using Microsoft.Owin.Host.HttpListener;
using System.Web.Http.Routing;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.Cors;

namespace RuiJi.Net.Owin
{
    public class Startup
    {
        static Startup()
        {
            OwinServerFactory.Initialize(new Dictionary<string, object>());
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage(new ErrorPageOptions()
            {
                ShowSourceCode = true,
                ShowEnvironment = true,
                ShowExceptionDetails = true,
                ShowQuery = true
            });

            app.UseCors(CorsOptions.AllowAll);

            var config = GetWebApiConfig();

            app.UseWebApi(config);

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\www")
            });
        }

        private HttpConfiguration GetWebApiConfig()
        {

            HttpConfiguration config = new HttpConfiguration();

            //config.Routes.IgnoreRoute("js", "{anything}.js/{*pathInfo}");

            #region Crawler Api
            config.Routes.MapHttpRoute(
                name: "Crawl",
                routeTemplate: "api/crawl",
                defaults: new { controller = "CrawlerApi", action = "crawl" }
            );

            config.Routes.MapHttpRoute(
                name: "ServerInfo",
                routeTemplate: "api/crawler/info",
                defaults: new { controller = "CrawlerApi", action = "serverinfo" }
            );

            config.Routes.MapHttpRoute(
                name: "CrawlerIps",
                routeTemplate: "api/crawler/ips",
                defaults: new { controller = "CrawlerApi", action = "Ips" }
            );

            config.Routes.MapHttpRoute(
                name: "SetCrawlerIps",
                routeTemplate: "api/crawler/ips/set",
                defaults: new { controller = "CrawlerApi", action = "SetIps" }
            );
            #endregion

            #region Extracter Api
            config.Routes.MapHttpRoute(
                name: "Extract",
                routeTemplate: "api/extract",
                defaults: new { controller = "ExtracterApi", action = "extract" }
            );
            #endregion

            #region Proxy Api
            config.Routes.MapHttpRoute(
                    name: "ProxyRequest",
                    routeTemplate: "api/cp/crawl",
                    defaults: new { controller = "CrawlerProxyApi", action = "crawl" }
                );

            config.Routes.MapHttpRoute(
                    name: "ProxyCrawlers",
                    routeTemplate: "api/cp/crawlers",
                    defaults: new { controller = "CrawlerProxyApi", action = "Crawlers" }
                );

            config.Routes.MapHttpRoute(
                name: "ProxyExtract",
                routeTemplate: "api/ep/extract",
                defaults: new { controller = "ExtracterProxyApi", action = "Extract" }
            );

            config.Routes.MapHttpRoute(
                name: "PingCP",
                routeTemplate: "api/cp/ping",
                defaults: new { controller = "CrawlerProxyApi", action = "Ping" }
            );

            config.Routes.MapHttpRoute(
                name: "PingEP",
                routeTemplate: "api/ep/ping",
                defaults: new { controller = "CrawlerProxyApi", action = "Ping" }
            );
            #endregion

            #region Zoo Api
            config.Routes.MapHttpRoute(
                    name: "ZooTree",
                    routeTemplate: "api/zoo/tree",
                    defaults: new { controller = "ZooApi", action = "Tree" }
                );

            config.Routes.MapHttpRoute(
                name: "ZooNodeData",
                routeTemplate: "api/zoo/node",
                defaults: new { controller = "ZooApi", action = "NodeData" }
            );

            config.Routes.MapHttpRoute(
                name: "NodeCluster",
                routeTemplate: "api/zoo/cluster",
                defaults: new { controller = "ZooApi", action = "Cluster" }
            );

            config.Routes.MapHttpRoute(
                name: "FeedProxyUrl",
                routeTemplate: "api/zoo/feedproxy",
                defaults: new { controller = "ZooApi", action = "FeedProxy" }
            );
            #endregion

            #region Feed Api
            config.Routes.MapHttpRoute(
                    name: "FeedList",
                    routeTemplate: "api/feeds",
                    defaults: new { controller = "FeedApi", action = "Feeds" }
                );

            config.Routes.MapHttpRoute(
                    name: "RuleList",
                    routeTemplate: "api/rules",
                    defaults: new { controller = "FeedApi", action = "Rules" }
                );

            config.Routes.MapHttpRoute(
                    name: "UrlRule",
                    routeTemplate: "api/fp/rule",
                    defaults: new { controller = "FeedApi", action = "UrlRule" }
                );

            config.Routes.MapHttpRoute(
                    name: "GetFeedPage",
                    routeTemplate: "api/feed/page",
                    defaults: new { controller = "FeedApi", action = "GetFeedPage" }
                );

            config.Routes.MapHttpRoute(
                    name: "SetFeedPage",
                    routeTemplate: "api/feed/page/set",
                    defaults: new { controller = "FeedApi", action = "SetFeedPage" }
                );

            config.Routes.MapHttpRoute(
                    name: "FeedJob",
                    routeTemplate: "api/feed/job",
                    defaults: new { controller = "FeedApi", action = "FeedJob" }
                );

            config.Routes.MapHttpRoute(
                    name: "UpdateFeed",
                    routeTemplate: "api/feed/update",
                    defaults: new { controller = "FeedApi", action = "UpdateFeed" }
                );

            config.Routes.MapHttpRoute(
                    name: "GetFeed",
                    routeTemplate: "api/feed",
                    defaults: new { controller = "FeedApi", action = "GetFeed" }
                );

            config.Routes.MapHttpRoute(
                   name: "UpdateRule",
                   routeTemplate: "api/rule/update",
                   defaults: new { controller = "FeedApi", action = "UpdateRule" }
               );

            config.Routes.MapHttpRoute(
                    name: "GetRule",
                    routeTemplate: "api/rule",
                    defaults: new { controller = "FeedApi", action = "GetRule" }
                );

            config.Routes.MapHttpRoute(
                    name: "RunCrawl",
                    routeTemplate: "api/feed/crawl",
                    defaults: new { controller = "FeedApi", action = "RunCrawl" }
                );

            config.Routes.MapHttpRoute(
                name: "SaveContent",
                routeTemplate: "api/fp/content/save",
                defaults: new { controller = "FeedApi", action = "SaveContent" }
            );

            config.Routes.MapHttpRoute(
                    name: "GetContent",
                    routeTemplate: "api/fp/content",
                    defaults: new { controller = "FeedApi", action = "GetContent" }
                );
            #endregion

            #region Funcs Api
            config.Routes.MapHttpRoute(
                    name: "FuncsList",
                    routeTemplate: "api/funcs",
                    defaults: new { controller = "FeedApi", action = "Funcs" }
                );

            config.Routes.MapHttpRoute(
                    name: "UpdateFunc",
                    routeTemplate: "api/funcs/update",
                    defaults: new { controller = "FeedApi", action = "UpdateFunc" }
                );
            #endregion

            #region Info Api
            config.Routes.MapHttpRoute(
                   name: "SystemInfo",
                   routeTemplate: "api/info/system",
                   defaults: new { controller = "InfoApi", action = "System" }
               );

            config.Routes.MapHttpRoute(
                    name: "ServerTypeInfo",
                    routeTemplate: "api/info/server",
                    defaults: new { controller = "InfoApi", action = "Server" }
                );

            config.Routes.MapHttpRoute(
                    name: "DllInfo",
                    routeTemplate: "api/info/dll",
                    defaults: new { controller = "InfoApi", action = "Dll" }
                );
            #endregion

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "json", "application/json"));

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };

            return config;
        }
    }
}