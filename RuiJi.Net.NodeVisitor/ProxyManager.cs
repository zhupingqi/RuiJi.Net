using org.apache.zookeeper;
using org.apache.zookeeper.data;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;

namespace RuiJi.Net.NodeVisitor
{
    public class ProxyManager
    {
        private static ProxyManager _instance;

        protected ZooKeeper zooKeeper;
        private string zkServer;
        private bool force;
        private List<LiveProxy> proxys;

        static ProxyManager()
        {
            _instance = new ProxyManager();
        }

        private ProxyManager()
        {
            proxys = new List<LiveProxy>();
            zkServer = RuiJiConfiguration.ZkServer;

            Start();
        }

        ~ProxyManager()
        {
        }

        public static ProxyManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Start()
        {
            var resetEvent = new ManualResetEvent(false);
            var watcher = new SessionWatcher(this, resetEvent);
            try
            {
                zooKeeper = new ZooKeeper(zkServer, 15000, watcher);
                resetEvent.WaitOne();

                LoadLiveProxy();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void LoadLiveProxy()
        {
            proxys.Clear();
            try
            {
                var nodes = zooKeeper.getChildrenAsync("/live_nodes/proxy", new LiveProxyWatcher(this)).Result.Children;

                foreach (var node in nodes)
                {
                    var d = GetData("/live_nodes/proxy/" + node);

                    proxys.Add(new LiveProxy
                    {
                        BaseUrl = IPHelper.FixLocalUrl(node),
                        Type = LiveProxy.GetType(d)
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger("").Error(ex.Message);
            }
        }

        public virtual void Stop()
        {
            force = true;

            if (zooKeeper != null)
            {
                zooKeeper.closeAsync();
                zooKeeper = null;
            }
        }

        public string Elect(NodeProxyTypeEnum proxyType)
        {
            var p = proxys.Where(m => m.Type == proxyType).OrderBy(m => m.Counts).FirstOrDefault();
            if (p != null)
            {
                p.Counts++;
                return p.BaseUrl;
            }

            return null;
        }

        public string GetData(string path)
        {
            try
            {
                var stat = new Stat();

                var b = zooKeeper.getDataAsync(path, false).Result.Data;
                if (b == null)
                    b = new byte[0];

                return Encoding.UTF8.GetString(b);
            }
            catch
            {
            }

            return null;
        }

        class SessionWatcher : Watcher
        {
            ProxyManager manager;
            ManualResetEvent resetEvent;

            public SessionWatcher(ProxyManager manager, ManualResetEvent resetEvent)
            {
                this.manager = manager;
                this.resetEvent = resetEvent;
            }

            public override Task process(WatchedEvent @event)
            {
                if (@event.get_Type() == EventType.None)
                {
                    switch (@event.getState())
                    {
                        case KeeperState.Disconnected:
                            {
                                Console.WriteLine("disconnected with zookeeper server");
                                if (!manager.force)
                                    manager.Start();
                                manager.force = false;
                                break;
                            }
                        case KeeperState.Expired:
                            {
                                Console.WriteLine("connected expired! reconnect!");
                                manager.Start();
                                break;
                            }
                        case KeeperState.SyncConnected:
                            {
                                Console.WriteLine("zookeeper server connected!");
                                resetEvent.Set();
                                break;
                            }
                        case KeeperState.ConnectedReadOnly:
                            {
                                Console.WriteLine("zookeeper server ConnectedReadOnly!");
                                break;
                            }
                        case KeeperState.AuthFailed:
                            {
                                break;
                            }
                    }
                }

                return Task.CompletedTask;
            }
        }

        class LiveProxyWatcher : Watcher
        {
            ProxyManager manager;

            public LiveProxyWatcher(ProxyManager manager)
            {
                this.manager = manager;
            }

            public override Task process(WatchedEvent @event)
            {
                if (string.IsNullOrEmpty(@event.getPath()))
                    return Task.CompletedTask;

                switch (@event.get_Type())
                {
                    case EventType.NodeChildrenChanged:
                        {
                            manager.LoadLiveProxy();

                            break;
                        }
                }

                return Task.CompletedTask;
            }
        }
    }
}