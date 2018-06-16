using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net;
using RuiJi.Net.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Db;
using System.Diagnostics;

namespace RuiJi.Net.Owin.Controllers
{
    public class CrawlerProxyApiController : ApiController
    {
        [HttpPost]
        [NodeRoute(Target = NodeProxyTypeEnum.Crawler)]
        //[WebApiCacheAttribute(Duration = 10)]
        public Response Crawl(Request request)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                CrawlerElectResult result;

                if (!string.IsNullOrEmpty(request.Ip))
                {
                    result = CrawlerManager.Instance.GetServer(request.Ip);
                }
                else
                {
                    result = CrawlerManager.Instance.ElectIP(request.Uri);
                    if (result == null)
                    {
                        return new Response
                        {
                            StatusCode = System.Net.HttpStatusCode.Conflict,
                            Data = "no clrawler ip elect!"
                        };
                    }
                    request.Ip = result.ClientIp;
                }

                request.Elect = result.BaseUrl;

                var p = ProxyLiteDb.Get(request.Uri.Scheme);
                if(p != null)
                {
                    request.Proxy = new RequestProxy();
                    request.Proxy.Host = (p.Type == Node.Db.ProxyTypeEnum.HTTP ? "https://" : "http://") + p.Ip;
                    request.Proxy.Port = p.Port;
                    request.Proxy.Username = p.UserName;
                    request.Proxy.Password = p.Password;
                }

                var client = new RestClient("http://" + result.BaseUrl);
                var restRequest = new RestRequest("api/crawl");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);
                response.ElectInfo = result.BaseUrl + "/" + result.ClientIp;

                if (p != null)
                {
                    response.Proxy = request.Proxy.Host;
                }

                return response;
            }
            else
            {
                return Crawler.Request(request);
            }
        }

        [HttpGet]
        public object Crawlers()
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                return CrawlerManager.Instance.ServerMap;
            }
            return new { };
        }

        [HttpPost]
        [WebApiCacheAttribute(Duration = 0)]
        [NodeRoute(Target = NodeProxyTypeEnum.Crawler)]
        public object Elect(CrawlerElectRequest request)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var result = new CrawlerElectResult();

            if (request.ElectIp)
            {
                result = CrawlerManager.Instance.ElectIP(request.Uri);
                if (result == null)
                    return new Response
                    {
                        StatusCode = System.Net.HttpStatusCode.Conflict,
                        Data = "no clrawler ip elect!"
                    };
            }

            if (request.ElectProxy)
            {
                var p = ProxyLiteDb.Get(request.Uri.Scheme);
                if (p != null)
                {
                    result.Proxy = new RequestProxy();
                    result.Proxy.Host = (p.Type == Node.Db.ProxyTypeEnum.HTTP ? "https://" : "http://") + p.Ip;
                    result.Proxy.Port = p.Port;
                    result.Proxy.Username = p.UserName;
                    result.Proxy.Password = p.Password;
                }
            }

            return result;
        }
    }
}