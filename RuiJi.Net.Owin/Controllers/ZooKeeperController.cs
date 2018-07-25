using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RuiJi.Net.Owin.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/zk")]
    public class ZooKeeperController : ControllerBase
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
        //    var auth = Request.Host.Value;
        //    var leaderNode = ServerManager.GetLeader();

        //    if (leaderNode != null && leaderNode.BaseUrl == auth)
        //    {
        //        return leaderNode;
        //    }

        //    return null;
        //}
    }
}