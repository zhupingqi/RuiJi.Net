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
                    request.Ip = result.ClientIp;
                }
                else
                {
                    result = CrawlerManager.Instance.ElectIP(request.Uri);
                    if (result == null)
                        return new Response
                        {
                            StatusCode = System.Net.HttpStatusCode.Conflict,
                            Data = "no clrawler ip elect!"
                        };
                }

                var p = ProxyLiteDb.Get();
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
                return new Crawler().Request(request);
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
        public object Elect(CrawlerElectRequest request)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                var result =  new CrawlerElectResult();

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
                    var p = ProxyLiteDb.Get();
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
            else
            {
                return new Crawler().Elect(request);
            }
        }

        #region Proxys
        [HttpGet]
        public object Proxys(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLERPROXY)
            {
                var paging = new Paging();
                paging.CurrentPage = (offset / limit) + 1;
                paging.PageSize = limit;

                return new
                {
                    rows = ProxyLiteDb.GetModels(paging),
                    total = paging.Count
                };
            }

            return new { };
        }

        [HttpPost]
        public object UpdateProxy(ProxyModel proxy)
        {
            ProxyLiteDb.AddOrUpdate(proxy);

            return true;
        }

        [HttpGet]
        public object GetProxy(int id)
        {
            var feed = ProxyLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        public bool RemoveProxy(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return ProxyLiteDb.Remove(removes);
        }

        [HttpGet]
        public int ProxyPing(int id)
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();

                var crawler = new RuiJiCrawler();
                var request = new Request("http://2017.ip138.com/ic.asp");

                var proxy = ProxyLiteDb.Get(id);
                var host = (proxy.Type == Node.Db.ProxyTypeEnum.HTTP ? "http" : "https") + proxy.Ip;
                request.Proxy = new RequestProxy(host, proxy.Port, proxy.UserName, proxy.Password);

                var response = crawler.Request(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    watch.Stop();
                    return watch.Elapsed.Milliseconds;
                }
            }
            catch
            {
                return -2;
            }

            return -1;
        }
        #endregion
    }
}