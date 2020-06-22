using org.apache.zookeeper;
using org.apache.zookeeper.data;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static org.apache.zookeeper.KeeperException;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;

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
                return zooKeeper.getState().ToString();
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

                zooKeeper = new ZooKeeper(ZkServer, 30000, watcher);
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
                zooKeeper.closeAsync();
                zooKeeper = null;
            }
        }

        protected abstract void OnStartup();

        protected abstract NodeTypeEnum SetNodeType();

        protected void CreateCommonNode()
        {
            //live_nodes node
            var stat = zooKeeper.existsAsync("/live_nodes", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/live_nodes/crawler", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/live_nodes/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/live_nodes/extractor", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/live_nodes/extractor", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/live_nodes/feed", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/live_nodes/feed", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/live_nodes/proxy", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/live_nodes/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            //config node
            stat = zooKeeper.existsAsync("/config", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/config", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/config/crawler", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/config/crawler", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/config/extractor", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/config/extractor", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/config/proxy", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/config/proxy", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/config/feed", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/config/feed", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/overseer", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/overseer", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            stat = zooKeeper.existsAsync("/overseer/election", false).Result;
            if (stat == null)
                zooKeeper.createAsync("/overseer/election", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
        }

        protected void CreateLiveNode(string path, byte[] data)
        {
            var stat = zooKeeper.existsAsync(path, false).Result;
            if (stat != null)
                zooKeeper.deleteAsync(path, -1);

            zooKeeper.createAsync(path, data, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
        }

        private void CreateOverseerNode()
        {
            var nodes = zooKeeper.getChildrenAsync("/overseer/election", null).Result.Children;
            if (nodes.Count(m => m.IndexOf(BaseUrl) != -1) > 0)
                return;
            zooKeeper.createAsync("/overseer/election/" + BaseUrl + "_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
        }

        protected void RunForLeaderNode()
        {
            try
            {
                var stat = zooKeeper.existsAsync("/overseer/leader", new LeaderNodeDeleteWatcher(this)).Result;
                if (stat == null)
                {
                    zooKeeper.createAsync("/overseer/leader", Encoding.UTF8.GetBytes( BaseUrl), Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
                    IsLeader = true;
                    LeaderBaseUrl = GetLeader();

                    Logger.GetLogger(BaseUrl).Info("current leader is " + BaseUrl);
                }
            }
            catch (NodeExistsException ex)
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
                var stat = new Stat();
                var b = zooKeeper.getDataAsync("/overseer/leader", false).Result.Data;
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

                var nodes = zooKeeper.getChildrenAsync(path, null).Result.Children;
                foreach (var node in nodes)
                {
                    var n = path == "/" ? path + node : path + "/" + node;

                    var stat = zooKeeper.existsAsync(n, false).Result;
                    nv.Add(n, stat.getNumChildren().ToString());
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
                zooKeeper.setDataAsync(path, Encoding.UTF8.GetBytes(data), version);
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

                var b = zooKeeper.getDataAsync(path, false).Result.Data;
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

        class SessionWatcher : Watcher
        {
            NodeBase service;
            ManualResetEvent resetEvent;

            public SessionWatcher(NodeBase service, ManualResetEvent resetEvent)
            {
                this.service = service;
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
                        case KeeperState.ConnectedReadOnly:
                            {
                                Logger.GetLogger(service.BaseUrl).Info("zookeeper server ConnectedReadOnly!");
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

        class LeaderNodeDeleteWatcher : Watcher
        {
            NodeBase service;
            public LeaderNodeDeleteWatcher(NodeBase service)
            {
                this.service = service;
            }

            public override Task process(WatchedEvent @event)
            {
                if (@event.get_Type() == EventType.NodeDeleted)
                {
                    Logger.GetLogger(service.BaseUrl).Error("leader offline, run for leader");
                    service.RunForLeaderNode();
                }

                return Task.CompletedTask;
            }
        }
    }
}