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
using System.Web.Http.Dispatcher;

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

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                RequestPath = new PathString("/download"),
                FileSystem = new PhysicalFileSystem(@".\www\download"),
            });
        }

        private HttpConfiguration GetWebApiConfig()
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.IgnoreRoute("js", "{anything}.js/{*pathInfo}");

           // #region Crawler Api
           // config.Routes.MapHttpRoute(
           //     name: "Crawl",
           //     routeTemplate: "api/crawl",
           //     defaults: new { controller = "CrawlerApi", action = "crawl" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "ServerInfo",
           //     routeTemplate: "api/crawler/info",
           //     defaults: new { controller = "CrawlerApi", action = "serverinfo" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "CrawlerIps",
           //     routeTemplate: "api/crawler/ips",
           //     defaults: new { controller = "CrawlerApi", action = "Ips" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "SetCrawlerIps",
           //     routeTemplate: "api/crawler/ips/set",
           //     defaults: new { controller = "CrawlerApi", action = "SetIps" }
           // );
           // #endregion

           // #region Extractor Api
           // config.Routes.MapHttpRoute(
           //     name: "Extract",
           //     routeTemplate: "api/extract",
           //     defaults: new { controller = "ExtractorApi", action = "extract" }
           // );
           // #endregion

           // #region Proxy Api
           // config.Routes.MapHttpRoute(
           //         name: "ProxyRequest",
           //         routeTemplate: "api/cp/crawl",
           //         defaults: new { controller = "CrawlerProxyApi", action = "crawl" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "ProxyElect",
           //         routeTemplate: "api/cp/elect",
           //         defaults: new { controller = "CrawlerProxyApi", action = "Elect" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "ProxyCrawlers",
           //         routeTemplate: "api/cp/crawlers",
           //         defaults: new { controller = "CrawlerProxyApi", action = "Crawlers" }
           //     );

           // config.Routes.MapHttpRoute(
           //     name: "ProxyExtract",
           //     routeTemplate: "api/ep/extract",
           //     defaults: new { controller = "ExtractorProxyApi", action = "Extract" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "PingCP",
           //     routeTemplate: "api/cp/ping",
           //     defaults: new { controller = "CrawlerProxyApi", action = "Ping" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "PingEP",
           //     routeTemplate: "api/ep/ping",
           //     defaults: new { controller = "CrawlerProxyApi", action = "Ping" }
           // );
           // #endregion

           // #region Zoo Api
           // config.Routes.MapHttpRoute(
           //         name: "ZooTree",
           //         routeTemplate: "api/zoo/tree",
           //         defaults: new { controller = "ZooApi", action = "Tree" }
           //     );

           // config.Routes.MapHttpRoute(
           //     name: "ZooNodeData",
           //     routeTemplate: "api/zoo/node",
           //     defaults: new { controller = "ZooApi", action = "NodeData" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "NodeCluster",
           //     routeTemplate: "api/zoo/cluster",
           //     defaults: new { controller = "ZooApi", action = "Cluster" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "FeedProxyUrl",
           //     routeTemplate: "api/zoo/proxys",
           //     defaults: new { controller = "ZooApi", action = "GetProxys" }
           // );
           // #endregion

           // #region Rule Api
           // config.Routes.MapHttpRoute(
           //     name: "UrlRule",
           //     routeTemplate: "api/fp/rule",
           //     defaults: new { controller = "FeedApi", action = "UrlRule" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "RuleList",
           //     routeTemplate: "api/rules",
           //     defaults: new { controller = "FeedApi", action = "Rules" }
           // );

           // config.Routes.MapHttpRoute(
           //    name: "UpdateRule",
           //    routeTemplate: "api/rule/update",
           //    defaults: new { controller = "FeedApi", action = "UpdateRule" }
           //);

           // config.Routes.MapHttpRoute(
           //     name: "TestRule",
           //     routeTemplate: "api/rule/test",
           //     defaults: new { controller = "FeedApi", action = "TestRule" }
           // );

           // config.Routes.MapHttpRoute(
           //         name: "GetRule",
           //         routeTemplate: "api/rule",
           //         defaults: new { controller = "FeedApi", action = "GetRule" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveRule",
           //         routeTemplate: "api/rule/remove",
           //         defaults: new { controller = "FeedApi", action = "RemoveRule" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RuleStatusChange",
           //         routeTemplate: "api/rule/status/change",
           //         defaults: new { controller = "FeedApi", action = "RuleStatusChange" }
           //     );
           // #endregion

           // #region Feed Api
           // //config.Routes.MapHttpRoute(
           // //        name: "FeedList",
           // //        routeTemplate: "api/feeds",
           // //        defaults: new { controller = "FeedApi", action = "Feeds" }
           // //    );

           // //config.Routes.MapHttpRoute(
           // //        name: "GetFeedPage",
           // //        routeTemplate: "api/feed/page",
           // //        defaults: new { controller = "FeedApi", action = "GetFeedPage" }
           // //    );

           // //config.Routes.MapHttpRoute(
           // //        name: "SetFeedPage",
           // //        routeTemplate: "api/feed/page/set",
           // //        defaults: new { controller = "FeedApi", action = "SetFeedPage" }
           // //    );

           // config.Routes.MapHttpRoute(
           //         name: "FeedJob",
           //         routeTemplate: "api/feed/job",
           //         defaults: new { controller = "FeedApi", action = "FeedJob" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "UpdateFeed",
           //         routeTemplate: "api/feed/update",
           //         defaults: new { controller = "FeedApi", action = "UpdateFeed" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "GetFeed",
           //         routeTemplate: "api/feed",
           //         defaults: new { controller = "FeedApi", action = "GetFeed" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RunCrawl",
           //         routeTemplate: "api/feed/crawl",
           //         defaults: new { controller = "FeedApi", action = "RunCrawl" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "TestFeed",
           //         routeTemplate: "api/feed/test",
           //         defaults: new { controller = "FeedApi", action = "TestFeed" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveFeeds",
           //         routeTemplate: "api/feed/remove",
           //         defaults: new { controller = "FeedApi", action = "RemoveFeed" }
           //     );

           // config.Routes.MapHttpRoute(
           //       name: "FeedStatusChange",
           //       routeTemplate: "api/feed/status/change",
           //       defaults: new { controller = "FeedApi", action = "FeedStatusChange" }
           //   );
           // #endregion

           // #region Content Api
           // config.Routes.MapHttpRoute(
           //     name: "SaveContent",
           //     routeTemplate: "api/fp/content/save",
           //     defaults: new { controller = "FeedApi", action = "SaveContent" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "GetContent",
           //     routeTemplate: "api/fp/content",
           //     defaults: new { controller = "FeedApi", action = "GetContent" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "GetShards",
           //     routeTemplate: "api/fp/content/shards",
           //     defaults: new { controller = "FeedApi", action = "GetShards" }
           // );

           // config.Routes.MapHttpRoute(
           //     name: "RemoveContent",
           //     routeTemplate: "api/fp/content/remove",
           //     defaults: new { controller = "FeedApi", action = "RemoveContent" }
           // );


           // #endregion

           // #region Funcs Api
           // config.Routes.MapHttpRoute(
           //         name: "FuncsList",
           //         routeTemplate: "api/funcs",
           //         defaults: new { controller = "SettingApi", action = "Funcs" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "GetFunc",
           //         routeTemplate: "api/func",
           //         defaults: new { controller = "SettingApi", action = "GetFunc" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "UpdateFunc",
           //         routeTemplate: "api/func/update",
           //         defaults: new { controller = "SettingApi", action = "UpdateFunc" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveFunc",
           //         routeTemplate: "api/func/remove",
           //         defaults: new { controller = "SettingApi", action = "RemoveFunc" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "FuncTest",
           //         routeTemplate: "api/func/test",
           //         defaults: new { controller = "SettingApi", action = "FuncTest" }
           //     );
           // #endregion

           // #region Info Api
           // config.Routes.MapHttpRoute(
           //        name: "SystemInfo",
           //        routeTemplate: "api/info/system",
           //        defaults: new { controller = "InfoApi", action = "System" }
           //    );

           // config.Routes.MapHttpRoute(
           //         name: "ServerTypeInfo",
           //         routeTemplate: "api/info/server",
           //         defaults: new { controller = "InfoApi", action = "Server" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "DllInfo",
           //         routeTemplate: "api/info/dll",
           //         defaults: new { controller = "InfoApi", action = "Dll" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "GitPulse",
           //         routeTemplate: "api/git/pulse",
           //         defaults: new { controller = "InfoApi", action = "Pulse" }
           //     );
           // #endregion

           // config.Routes.MapHttpRoute(
           //         name: "Logger",
           //         routeTemplate: "api/log",
           //         defaults: new { controller = "LogApi", action = "Log" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "Alone",
           //         routeTemplate: "api/alone",
           //         defaults: new { controller = "SettingApi", action = "IsAlone" }
           //     );

           // #region Online Proxy
           // config.Routes.MapHttpRoute(
           //     name: "ProxysList",
           //     routeTemplate: "api/proxies",
           //     defaults: new { controller = "SettingApi", action = "Proxys" }
           // );

           // config.Routes.MapHttpRoute(
           //         name: "GetProxy",
           //         routeTemplate: "api/proxy",
           //         defaults: new { controller = "SettingApi", action = "GetProxy" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "UpdateProxy",
           //         routeTemplate: "api/proxy/update",
           //         defaults: new { controller = "SettingApi", action = "UpdateProxy" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveProxy",
           //         routeTemplate: "api/proxy/remove",
           //         defaults: new { controller = "SettingApi", action = "RemoveProxy" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "ProxyStatusChange",
           //         routeTemplate: "api/proxy/status/change",
           //         defaults: new { controller = "SettingApi", action = "ProxyStatusChange" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "ProxyPing",
           //         routeTemplate: "api/proxy/ping",
           //         defaults: new { controller = "SettingApi", action = "ProxyPing" }
           //     );
           // #endregion

           // #region UAGroup Api
           // config.Routes.MapHttpRoute(
           //        name: "UAGroupList",
           //        routeTemplate: "api/ua/groups",
           //        defaults: new { controller = "SettingApi", action = "UAGroups" }
           //    );
           // config.Routes.MapHttpRoute(
           //       name: "UAGroup",
           //       routeTemplate: "api/ua/group",
           //       defaults: new { controller = "SettingApi", action = "UAGroup" }
           //   );

           // config.Routes.MapHttpRoute(
           //         name: "UpdateUAGroup",
           //         routeTemplate: "api/ua/group/update",
           //         defaults: new { controller = "SettingApi", action = "UpdateUAGroup" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveUAGroup",
           //         routeTemplate: "api/ua/group/remove",
           //         defaults: new { controller = "SettingApi", action = "RemoveUAGroup" }
           //     );
           // #endregion

           // #region UA Api

           // config.Routes.MapHttpRoute(
           //        name: "UAList",
           //        routeTemplate: "api/uas",
           //        defaults: new { controller = "SettingApi", action = "UAs" }
           //    );

           // config.Routes.MapHttpRoute(
           //         name: "UA",
           //         routeTemplate: "api/ua",
           //         defaults: new { controller = "SettingApi", action = "UA" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "UpdateUA",
           //         routeTemplate: "api/ua/update",
           //         defaults: new { controller = "SettingApi", action = "UpdateUA" }
           //     );

           // config.Routes.MapHttpRoute(
           //         name: "RemoveUAs",
           //         routeTemplate: "api/ua/remove",
           //         defaults: new { controller = "SettingApi", action = "RemoveUAs" }
           //     );
           // #endregion

           // #region Setting Node Api
           // config.Routes.MapHttpRoute(
           //         name: "GetNode",
           //         routeTemplate: "api/nodes",
           //         defaults: new { controller = "SettingApi", action = "Nodes" }
           //     );
           // #endregion

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };

            config.EnsureInitialized();

            return config;
        }
    }
}