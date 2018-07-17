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

            var leaderNode = ServerManager.ZkNode();

            if (leaderNode != null)
            {
                var nv = leaderNode.GetChildren(path);

                return nv.AllKeys.Select(m => new { id = m ,text = m.LastIndexOf('/') !=-1 ? m.Substring(m.LastIndexOf('/') + 1) : m, children = nv[m] != "0" });
            }

            return new object(); 
        }

        [HttpGet]
        [Route("data")]
        public object NodeData(string path)
        {
            var leaderNode = ServerManager.ZkNode();

            if (leaderNode != null)
            {
                return leaderNode.GetData(path);
            }

            return new object();
        }

        [HttpGet]
        [Route("cluster")]
        public object Cluster()
        {
            var leaderNode = ServerManager.ZkNode();

            if (leaderNode!=null)
            {
                return leaderNode.GetCluster();
            }

            return new object();
        }

        //private NodeBase GetLeaderNode()
        //{
        //    var auth = Request.RequestUri.Authority;
        //    var leaderNode = ServerManager.GetLeader();

        //    if (leaderNode != null && leaderNode.BaseUrl == auth)
        //    {
        //        return leaderNode;
        //    }

        //    return null;
        //}
    }
}