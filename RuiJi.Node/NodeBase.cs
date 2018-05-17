using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Node
{
    public abstract class NodeBase
    {
        protected ZooKeeper ZooKeeper { get; private set; }

        public string ZkServer { get; protected set; }

        public string BaseUrl { get; protected set; }

        public string ProxyUrl { get; protected set; }

        public string States
        {
            get
            {
                return ZooKeeper.State.State;
            }
        }

        public ManualResetEvent ResetEvent { get; protected set; }

        public NodeBase(string baseUrl, string zkServer,string proxyUrl = "")
        {
            this.BaseUrl = IPHelper.FixLocalUrl(baseUrl);
            this.ZkServer = IPHelper.FixLocalUrl(zkServer);
            this.ProxyUrl = IPHelper.FixLocalUrl(proxyUrl);
        }

        public virtual void Start()
        {
            if (string.IsNullOrEmpty(BaseUrl) || string.IsNullOrEmpty(ZkServer))
                throw new Exception("BaseUrl and ZkServer must be set,call setup method!");

            ResetEvent = new ManualResetEvent(false);

            try
            {
                Console.WriteLine("proxy " + BaseUrl + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + ZkServer);

                var watcher = new SessionWatcher(this);

                ZooKeeper = new ZooKeeper(ZkServer, TimeSpan.FromSeconds(3), watcher);
                ResetEvent.WaitOne();

                CreateCommonNode();
                OnStartup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public virtual void Stop()
        {
            if (ZooKeeper != null)
            {
                ZooKeeper.Dispose();
                ZooKeeper = null;
            }
        }

        protected virtual void OnStartup()
        {

        }

        protected void CreateCommonNode()
        {
            //live_nodes node
            var stat = ZooKeeper.Exists("/live_nodes", false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/live_nodes/crawler", false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/live_nodes/extracter", false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes/extracter", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/live_nodes/proxy", false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            //config node
            stat = ZooKeeper.Exists("/config", false);
            if (stat == null)
                ZooKeeper.Create("/config", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/config/crawler", false);
            if (stat == null)
                ZooKeeper.Create("/config/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/config/extracter", false);
            if (stat == null)
                ZooKeeper.Create("/config/extracter", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = ZooKeeper.Exists("/config/proxy", false);
            if (stat == null)
                ZooKeeper.Create("/config/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
        }

        protected virtual void Process(WatchedEvent @event)
        {

        }

        class SessionWatcher : IWatcher
        {
            NodeBase service;

            public SessionWatcher(NodeBase service)
            {
                this.service = service;
            }

            public void Process(WatchedEvent @event)
            {
                if (@event.Type == EventType.None)
                {
                    switch (@event.State)
                    {
                        case KeeperState.Disconnected:
                            {
                                Console.WriteLine("disconnected with zookeeper server");
                                break;
                            }
                        case KeeperState.Expired:
                            {
                                Console.WriteLine("connected expired! reconnect!");
                                service.Start();
                                break;
                            }
                        case KeeperState.SyncConnected:
                            {
                                Console.WriteLine("zookeeper server connected!");
                                service.ResetEvent.Set();
                                break;
                            }
                        case KeeperState.NoSyncConnected:
                            {
                                Console.WriteLine("zookeeper server NoSyncConnected!");
                                break;
                            }
                        case KeeperState.Unknown:
                            {
                                break;
                            }
                    }
                }

                service.Process(@event);
            }
        }
    }
}