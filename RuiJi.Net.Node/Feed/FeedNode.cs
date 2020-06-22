using Newtonsoft.Json;
using org.apache.zookeeper;
using RuiJi.Net.Node.Feed.LTS;
using System.Text;
using static org.apache.zookeeper.ZooDefs;

namespace RuiJi.Net.Node.Feed
{
    public class FeedNode : NodeBase
    {
        private FeedScheduler scheduler;

        public FeedNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {
        }

        ~FeedNode()
        {
            FeedScheduler.GetSecheduler(BaseUrl).Stop();
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/feed/" + BaseUrl, null);

            var stat = zooKeeper.existsAsync("/config/feed/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new NodeConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl
                };
                zooKeeper.createAsync("/config/feed/" + BaseUrl, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }

            scheduler = new FeedScheduler();
            scheduler.Start(BaseUrl, this);
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.FEED;
        }

        public override void Stop()
        {
            if (scheduler != null)
            {
                scheduler.Stop();
                scheduler = null;
            }

            base.Stop();
        }
    }
}