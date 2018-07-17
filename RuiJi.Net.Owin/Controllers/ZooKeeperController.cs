using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RuiJi.Net.Node;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    [RoutePrefix("api/zk")]
    public class ZooKeeperController : ApiController
    {
        [HttpGet]
        [Route("tree")]
        public object Tree(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = "/";

            var leaderNode = GetLeaderNode();

            if (leaderNode != null)
            {
                var nv = leaderNode.GetChildren(path);

                return nv.AllKeys.Select(m => new { id = m ,text = m.LastIndexOf('/') !=-1 ? m.Substring(m.LastIndexOf('/') + 1) : m, children = nv[m] != "0" });
            }
            else
            {
                var leaderBaseUrl = (ServerManager.Get(Request.RequestUri.Authority) as NodeBase).LeaderBaseUrl;

                if (string.IsNullOrEmpty(leaderBaseUrl))
                    return null;

                var client = new RestClient("http://" + leaderBaseUrl);
                var restRequest = new RestRequest("api/zk/tree?path=" + path);
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }

        [HttpGet]
        [Route("data")]
        public object NodeData(string path)
        {
            var leaderNode = GetLeaderNode();

            if (leaderNode != null)
            {
                return leaderNode.GetData(path);
            }
            else
            {
                var leaderBaseUrl = (ServerManager.Get(Request.RequestUri.Authority) as NodeBase).LeaderBaseUrl;

                var client = new RestClient("http://" + leaderBaseUrl);
                var restRequest = new RestRequest("api/zk/data?path=" + path);
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }

        [HttpGet]
        [Route("cluster")]
        public object Cluster()
        {
            var leaderNode = GetLeaderNode();

            if (leaderNode!=null)
            {
                return leaderNode.GetCluster();
            }
            else
            {
                var leaderBaseUrl = (ServerManager.Get(Request.RequestUri.Authority) as NodeBase).LeaderBaseUrl;

                var client = new RestClient("http://" + leaderBaseUrl);
                var restRequest = new RestRequest("api/zk/cluster");
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<object>(restResponse.Content);

                return response;
            }
        }

        private NodeBase GetLeaderNode()
        {
            var auth = Request.RequestUri.Authority;
            var leaderNode = ServerManager.GetLeader();

            if (leaderNode != null && leaderNode.BaseUrl == auth)
            {
                return leaderNode;
            }

            return null;
        }
    }
}