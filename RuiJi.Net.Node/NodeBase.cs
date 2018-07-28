using log4net;
using Newtonsoft.Json;
using Org.Apache.Zookeeper.Data;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Net.Node
{
    public abstract class NodeBase : INode
    {
        protected ZooKeeper zooKeeper;

        public string ZkServer { get; protected set; }

        public string BaseUrl { get; protected set; }

        public string ProxyUrl { get; protected set; }

        public string LeaderBaseUrl { get; protected set; }

        public NodeTypeEnum NodeType { get; private set; }

        public DateTime StartTime { get; private set; }

        private bool force;

        public string States
        {
            get
            {
                return zooKeeper.State.State;
            }
        }

        public bool IsLeader { get; private set; }

        public NodeBase(string baseUrl, string zkServer, string proxyUrl = "")
        {
            this.BaseUrl = IPHelper.FixLocalUrl(baseUrl);
            this.ZkServer = IPHelper.FixLocalUrl(zkServer);
            this.ProxyUrl = IPHelper.FixLocalUrl(proxyUrl);
            this.StartTime = DateTime.Now;
            this.NodeType = SetNodeType();
        }

        public virtual void Start()
        {
            if (string.IsNullOrEmpty(BaseUrl) || string.IsNullOrEmpty(ZkServer))
            {
                Logger.GetLogger(BaseUrl).Fatal("BaseUrl and ZkServer must be set,call setup method!");
                throw new Exception("BaseUrl and ZkServer must be set,call setup method!");
            }

            Logger.Add(BaseUrl, new List<IAppender> {
                new RollingFileAppender(BaseUrl),
                new MemoryAppender(),
                new ConsoleAppender()
            });

            Logger.GetLogger(BaseUrl).Info("Start WebApiServer At http://" + BaseUrl + " with " + NodeType.ToString() + " node");

            var resetEvent = new ManualResetEvent(false);
            var watcher = new SessionWatcher(this, resetEvent);

            try
            {
                Logger.GetLogger(BaseUrl).Info("node " + BaseUrl + " ready to startup!");
                Logger.GetLogger(BaseUrl).Info("try connect to zookeeper server : " + ZkServer);

                zooKeeper = new ZooKeeper(ZkServer, TimeSpan.FromSeconds(30), watcher);
                resetEvent.WaitOne();

                CreateCommonNode();
                RunForLeaderNode();
                CreateOverseerNode();
                OnStartup();
            }
            catch (Exception ex)
            {
                Logger.GetLogger(BaseUrl).Error(ex.Message);
            }
        }

        public virtual void Stop()
        {
            force = true;

            if (zooKeeper != null)
            {
                zooKeeper.Dispose();
                zooKeeper = null;
            }
        }

        protected abstract void OnStartup();

        protected abstract NodeTypeEnum SetNodeType();

        protected void CreateCommonNode()
        {
            //live_nodes node
            var stat = zooKeeper.Exists("/live_nodes", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/crawler", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/extractor", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/extractor", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/live_nodes/feed", false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/feed", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

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

            stat = zooKeeper.Exists("/config/extractor", false);
            if (stat == null)
                zooKeeper.Create("/config/extractor", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/config/proxy", false);
            if (stat == null)
                zooKeeper.Create("/config/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/config/feed", false);
            if (stat == null)
                zooKeeper.Create("/config/feed", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/overseer", false);
            if (stat == null)
                zooKeeper.Create("/overseer", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zooKeeper.Exists("/overseer/election", false);
            if (stat == null)
                zooKeeper.Create("/overseer/election", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
        }

        protected void CreateLiveNode(string path, byte[] data)
        {
            var stat = zooKeeper.Exists(path, false);
            if (stat != null)
                zooKeeper.Delete(path, -1);

            zooKeeper.Create(path, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
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
                    LeaderBaseUrl = GetLeader();

                    Logger.GetLogger(BaseUrl).Info("current leader is " + BaseUrl);
                }
            }
            catch (ZooKeeperNet.KeeperException.NodeExistsException ex)
            {
                IsLeader = false;
                LeaderBaseUrl = GetLeader();

                Logger.GetLogger(BaseUrl).Info(BaseUrl + " run for leader failed!" + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.GetLogger(BaseUrl).Error(ex.Message);
            }
        }

        private string GetLeader()
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
                Logger.GetLogger(BaseUrl).Error(BaseUrl + " read leader failed!" + ex.Message);
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

        public void SetData(string path, string data, int version = -1)
        {
            try
            {
                zooKeeper.SetData(path, data.GetBytes(), version);
            }
            catch
            {
            }
        }

        public NodeData GetData(string path)
        {
            try
            {
                var stat = new Stat();

                var b = zooKeeper.GetData(path, null, stat);
                if (b == null)
                    b = new byte[0];

                return new NodeData
                {
                    Stat = stat,
                    Data = Encoding.UTF8.GetString(b),
                    Path = path
                };
            }
            catch
            {
            }

            return null;
        }

        public List<NodeData> GetLiveNode()
        {
            var results = new List<NodeData>();
            var nodes = new List<string>();

            nodes.AddRange(GetChildren("/live_nodes/proxy").AllKeys);
            nodes.AddRange(GetChildren("/live_nodes/crawler").AllKeys);
            nodes.AddRange(GetChildren("/live_nodes/extractor").AllKeys);
            nodes.AddRange(GetChildren("/live_nodes/feed").AllKeys);

            foreach (var node in nodes)
            {
                var data = GetData(node);
                results.Add(data);
            }

            return results;
        }

        public List<NodeData> GetCluster()
        {
            var results = new List<NodeData>();

            var nodes = new List<string>();

            nodes.AddRange(GetChildren("/config/proxy").AllKeys);
            nodes.AddRange(GetChildren("/config/crawler").AllKeys);
            nodes.AddRange(GetChildren("/config/extractor").AllKeys);
            nodes.AddRange(GetChildren("/config/feed").AllKeys);

            foreach (var node in nodes)
            {
                var data = GetData(node);
                results.Add(data);
            }

            results.AddRange(GetLiveNode());
            return results;
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
                                Logger.GetLogger(service.BaseUrl).Error("disconnected with zookeeper server");
                                //wait 5 second.
                                Thread.Sleep(5000);
                                if (!service.force)
                                    service.Start();
                                service.force = false;
                                break;
                            }
                        case KeeperState.Expired:
                            {
                                Logger.GetLogger(service.BaseUrl).Error("connected expired! reconnect!");
                                service.Start();
                                break;
                            }
                        case KeeperState.SyncConnected:
                            {
                                Logger.GetLogger(service.BaseUrl).Info("zookeeper server connected!");
                                resetEvent.Set();
                                break;
                            }
                        case KeeperState.NoSyncConnected:
                            {
                                Logger.GetLogger(service.BaseUrl).Info("zookeeper server NoSyncConnected!");
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
                if (@event.Type == EventType.NodeDeleted)
                {
                    Logger.GetLogger(service.BaseUrl).Error("leader offline, run for leader");
                    service.RunForLeaderNode();
                }
            }
        }
    }
}