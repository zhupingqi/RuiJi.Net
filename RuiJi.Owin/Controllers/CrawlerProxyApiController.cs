using Newtonsoft.Json;
using RestSharp;
using RuiJi.Core.Crawler;
using RuiJi.Node.CrawlerProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class CrawlerProxyApiController : ApiController
    {
        [HttpPost]
        //[WebApiCacheAttribute(Duration = 10)]
        public Response Crawl(Request request)
        {
            var result = CrawlerManager.Instance.ElectIP(request.Uri);
            if (result == null)
                return new Response {
                    StatusCode = System.Net.HttpStatusCode.Conflict,
                    Data = "no clrawler ip elect!"
                };

            request.Ip = result.ClientIp;

            var client = new RestClient("http://" + result.BaseUrl);
            var restRequest = new RestRequest("api/crawl");
            restRequest.Method = Method.POST;
            restRequest.AddJsonBody(request);
            restRequest.Timeout = request.Timeout;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);

            return response;
        }

        [HttpGet]
        public bool Ping()
        {
            return true;
        }
    }
}
