using Newtonsoft.Json;
using org.apache.zookeeper;
using RuiJi.Net.Core.Utils.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;

namespace RuiJi.Net.Node.Extractor
{
    public class ExtractorProxyNode : NodeBase
    {
        public ExtractorProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {            
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, Encoding.UTF8.GetBytes("Extractor proxy"));

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.existsAsync("/config/proxy/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new 
                {
                    type = "Extractor"
                };

                zooKeeper.createAsync("/config/proxy/" + BaseUrl, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }

            LoadLiveExtractor();
        }

        public NodeConfig GetExtractorConfig(string baseUrl)
        {
            var b = zooKeeper.getDataAsync("/config/crawler/" + baseUrl, false).Result.Data;
            var r = System.Text.Encoding.UTF8.GetString(b);
            var d = JsonConvert.DeserializeObject<NodeConfig>(r);

            return d;
        }

        protected void LoadLiveExtractor()
        {
            try
            {
                ExtractorManager.Instance.Clear();

                var nodes = zooKeeper.getChildrenAsync("/live_nodes/extractor", new LiveExtractorWatcher(this)).Result.Children;

                ExtractorManager.Instance.ClearAndAddServer(nodes.ToArray());
            }
            catch
            {
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.EXTRACTORPROXY;
        }

        class LiveExtractorWatcher : Watcher
        {
            ExtractorProxyNode node;

            public LiveExtractorWatcher(ExtractorProxyNode node)
            {
                this.node = node;
            }

            public override Task process(WatchedEvent @event)
            {
                switch (@event.get_Type())
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveExtractor();
                            Logger.GetLogger(node.BaseUrl).Info("detected Extractor node change");
                            break;
                        }
                }

                return Task.CompletedTask;
            }
        }
    }
}