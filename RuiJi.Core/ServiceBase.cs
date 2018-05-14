using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Core
{
    public abstract class ServiceBase
    {
        protected ZooKeeper zookeeper;

        public string ZkServer { get; protected set; }

        public string BaseUrl { get; protected set; }

        public string ProxyUrl { get; protected set; }

        public string States
        {
            get
            {
                return zookeeper.State.State;
            }
        }

        public ManualResetEvent ResetEvent { get; protected set; }

        public abstract void Start();

        public abstract void Stop();

        public void Setup(string baseUrl, string zkServer,string proxyUrl = "")
        {
            this.BaseUrl = baseUrl;
            this.ZkServer = zkServer;
            this.ProxyUrl = proxyUrl;
        }

        protected void CreateCommonNode()
        {
            var stat = zookeeper.Exists("/live_nodes", false);
            if (stat == null)
                zookeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zookeeper.Exists("/config", false);
            if (stat == null)
                zookeeper.Create("/config", null, Ids.READ_ACL_UNSAFE, CreateMode.Persistent);
        }
    }
}