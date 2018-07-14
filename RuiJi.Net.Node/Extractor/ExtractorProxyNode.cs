using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node.Extractor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Net.Node.Extractor
{
    public class ExtractorProxyNode : NodeBase
    {
        public ExtractorProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {
            
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, "Extractor proxy".GetBytes());

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.Exists("/config/proxy/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new 
                {
                    type = "Extractor"
                };

                zooKeeper.Create("/config/proxy/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            LoadLiveExtractor();
        }

        public NodeConfig GetExtractorConfig(string baseUrl)
        {
            var b = zooKeeper.GetData("/config/crawler/" + baseUrl, false, null);
            var r = System.Text.Encoding.UTF8.GetString(b);
            var d = JsonConvert.DeserializeObject<NodeConfig>(r);

            return d;
        }

        protected void LoadLiveExtractor()
        {
            try
            {
                ExtractorManager.Instance.Clear();

                var nodes = zooKeeper.GetChildren("/live_nodes/extractor", new LiveExtractorWatcher(this));

                ExtractorManager.Instance.ClearAndAddServer(nodes.ToArray());
            }
            catch { }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.EXTRACTORPROXY;
        }

        class LiveExtractorWatcher : IWatcher
        {
            ExtractorProxyNode node;

            public LiveExtractorWatcher(ExtractorProxyNode node)
            {
                this.node = node;
            }

            public void Process(WatchedEvent @event)
            {
                switch (@event.Type)
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveExtractor();
                            Logger.GetLogger(node.BaseUrl).Info("detected Extractor node change");
                            break;
                        }
                }
            }
        }
    }
}