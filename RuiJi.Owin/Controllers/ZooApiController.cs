using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RuiJi.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class ZooApiController : ApiController
    {
        [HttpGet]
        public object Tree(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = "/";

            var auth = Request.RequestUri.Authority;
            var leaderNode = ServerManager.GetLeader();

            if (leaderNode == null)
                return new string[0];

            if (leaderNode.BaseUrl == auth)
            {
                var nv = leaderNode.GetChildren(path);

                return nv.AllKeys.Select(m => new { id = m ,text = m.LastIndexOf('/') !=-1 ? m.Substring(m.LastIndexOf('/') + 1) : m, children = nv[m] != "0" });
            }
            else
            {
                var client = new RestClient("http://" + leaderNode.BaseUrl);
                var restRequest = new RestRequest("api/zoo/tree?path=" + path);
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }

        [HttpGet]
        public object NodeData(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var auth = Request.RequestUri.Authority;
            var leaderNode = ServerManager.GetLeader();

            if (leaderNode == null)
                return new string[0];

            if (leaderNode.BaseUrl == auth)
            {
                return leaderNode.GetData(path);
            }
            else
            {
                var client = new RestClient("http://" + leaderNode.BaseUrl);
                var restRequest = new RestRequest("api/zoo/tree?path=" + path);
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }

        [HttpGet]
        public object Cluster()
        {
            var auth = Request.RequestUri.Authority;
            var leaderNode = ServerManager.GetLeader();

            if (leaderNode == null)
                return new { };

            if (leaderNode.BaseUrl == auth)
            {
                return leaderNode.GetCluster();
            }
            else
            {
                var client = new RestClient("http://" + leaderNode.BaseUrl);
                var restRequest = new RestRequest("api/zoo/cluster");
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }
    }
}