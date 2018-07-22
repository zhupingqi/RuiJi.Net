using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.NodeVisitor;
using System.Threading;

namespace RuiJi.Net.Owin.Controllers
{
    [Route("api/cp")]
    public class CrawlerProxyController : ControllerBase
    {
        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.CRAWLERPROXY)]
        [Route("request")]
        public Response Crawl([FromBody]Request request)
        {
            var node = ServerManager.Get(Request.Host.Value);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                CrawlerElectResult result;

                if (!string.IsNullOrEmpty(request.Ip))
                {
                    result = CrawlerServerManager.Instance.GetServer(request.Ip);
                }
                else
                {
                    result = CrawlerServerManager.Instance.ElectIP(request.Uri);
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
                if (p != null)
                {
                    request.Proxy = new RequestProxy();
                    request.Proxy.Ip = p.Ip;
                    request.Proxy.Port = p.Port;
                    request.Proxy.Scheme = p.Type.ToString().ToLower();
                    request.Proxy.Username = p.UserName;
                    request.Proxy.Password = p.Password;
                }

                var client = new RestClient("http://" + result.BaseUrl);
                var restRequest = new RestRequest("api/crawler/crawl");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                Response response = null;
                var resetEvent = new ManualResetEvent(false);

                var handle = client.ExecuteAsync(restRequest,(restResponse)=> {
                    response = JsonConvert.DeserializeObject<Response>(restResponse.Content);
                    response.ElectInfo = result.BaseUrl + "/" + result.ClientIp;
                    resetEvent.Set();
                });

                resetEvent.WaitOne();

                return response;
            }
            else
            {
                return Crawler.Request(request);
            }
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.CRAWLERPROXY)]
        [Route("crawlers")]
        public object Crawlers()
        {
            var node = ServerManager.Get(Request.Host.Value);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                return CrawlerServerManager.Instance.Servers;
            }
            return new { };
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.CRAWLERPROXY)]
        [Route("elect")]
        public object Elect([FromBody]CrawlerElectRequest request)
        {
            var result = new CrawlerElectResult();

            if (request.ElectIp)
            {
                result = CrawlerServerManager.Instance.ElectIP(request.Uri);
                if (result == null)
                    return new Response
                    {
                        StatusCode = System.Net.HttpStatusCode.Conflict,
                        Data = "no clrawler ip elect!"
                    };
            }

            if (request.ElectProxy)
            {
                //去fp拿
                var p = ProxyLiteDb.Get(request.Uri.Scheme);
                if (p != null)
                {
                    result.Proxy = new RequestProxy();
                    result.Proxy.Ip = p.Ip;
                    result.Proxy.Port = p.Port;
                    result.Proxy.Scheme = p.Type.ToString().ToLower();
                    result.Proxy.Username = p.UserName;
                    result.Proxy.Password = p.Password;
                }
            }

            return result;
        }
    }
}