using Newtonsoft.Json;
using Org.Apache.Zookeeper.Data;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Node
{
    public abstract class NodeBase
    {
        protected ZooKeeper zooKeeper;

        public string ZkServer { get; protected set; }

        public string BaseUrl { get; protected set; }

        public string ProxyUrl { get; protected set; }

        public string LeaderBaseUrl { get; protected set; }

        public string States
        {
            get
            {
                return zooKeeper.State.State;
            }
        }

        public bool IsLeader { get; private set; }

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

            var resetEvent = new ManualResetEvent(false);
            var watcher = new SessionWatcher(this, resetEvent);

            try
            {
                Console.WriteLine("node " + BaseUrl + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + ZkServer);                

                zooKeeper = new ZooKeeper(ZkServer, TimeSpan.FromSeconds(15), watcher);
                resetEvent.WaitOne();

                CreateCommonNode();
                RunForLeaderNode();
                CreateOverseerNode();
                OnStartup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public virtual void Stop()
        {
            if (zooKeeper != null)
            {
                zooKeeper.Dispose();
                zooKeeper = null;
            }
        }

        protected abstract void OnStartup();

        protected void CreateCommonNode()
        {
            //live_nodes node
            var stat = zooKeeper.Exists("/live_nodes", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/crawler", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/extracter", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/extracter", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/proxy", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            //config node
            stat = zooKeeper.Exists("/config", false);
            if (stat == null)
                zooKeeper.Create("/config", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/config/crawler", false);
            if (stat == null)
                zooKeeper.Create("/config/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/config/extracter", false);
            if (stat == null)
                zooKeeper.Create("/config/extracter", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/config/proxy", false);
            if (stat == null)
                zooKeeper.Create("/config/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent); 

            stat = zooKeeper.Exists("/overseer", false);
            if (stat == null)
                zooKeeper.Create("/overseer", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/overseer/election", false);
            if (stat == null)
                zooKeeper.Create("/overseer/election", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
        }

        private void CreateOverseerNode()
        {
            var nodes = zooKeeper.GetChildren("/overseer/election", null);
            if (nodes.Count(m => m.IndexOf(BaseUrl) != -1) > 0)
                return;
            zooKeeper.Create("/overseer/election/" + BaseUrl + "_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
        }

        protected void RunForLeaderNode()
        {
            try
            {
                var stat = zooKeeper.Exists("/overseer/leader", new LeaderNodeDeleteWatcher(this));
                if (stat == null)
                {
                    zooKeeper.Create("/overseer/leader", BaseUrl.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
                    IsLeader = true;
                    Console.WriteLine("current leader is " + BaseUrl);
                }
            }
            catch (ZooKeeperNet.KeeperException.NodeExistsException ex)
            {
                IsLeader = false;
                Console.WriteLine(BaseUrl + " run for leader failed!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            LeaderBaseUrl = GetLeader();
        }

        protected string GetLeader()
        {
            try
            {
                var stat = new Org.Apache.Zookeeper.Data.Stat();
                var b = zooKeeper.GetData("/overseer/leader", false, stat);
                if (b != null)
                {
                    return Encoding.UTF8.GetString(b);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(BaseUrl + " read leader failed!");                
            }

            return null;
        }

        public NameValueCollection GetChildren(string path)
        {
            try
            {
                var nv = new NameValueCollection();

                var nodes = zooKeeper.GetChildren(path, null);
                foreach (var node in nodes)
                {
                    var n = path == "/" ? path + node : path + "/" + node;

                    var stat = zooKeeper.Exists(n, false);
                    nv.Add(n, stat.NumChildren.ToString());
                }

                return nv;
            }
            catch
            {
                return new NameValueCollection();
            }
        }

        public void SetData(string path,string data)
        {
            zooKeeper.SetData(path, data.GetBytes(), 0);
        }

        class SessionWatcher : IWatcher
        {
            NodeBase service;
            ManualResetEvent resetEvent;

            public SessionWatcher(NodeBase service, ManualResetEvent resetEvent)
            {
                this.service = service;
                this.resetEvent = resetEvent;
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
                                resetEvent.Set();
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
            }
        }

        class LeaderNodeDeleteWatcher : IWatcher
        {
            NodeBase service;
            public LeaderNodeDeleteWatcher(NodeBase service)
            {
                this.service = service;
            }

            public void Process(WatchedEvent @event)
            {
                if(@event.Type == EventType.NodeDeleted)
                {
                    Console.WriteLine("leader offline, run for leader");
                    service.RunForLeaderNode();
                }
            }
        }
    }
}