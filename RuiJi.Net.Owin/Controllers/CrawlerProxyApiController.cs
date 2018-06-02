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

                request.Ip = result.ClientIp;

                var client = new RestClient("http://" + result.BaseUrl);
                var restRequest = new RestRequest("api/crawl");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);
                response.ElectInfo = result.BaseUrl + "/" + result.ClientIp;

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
    }
}