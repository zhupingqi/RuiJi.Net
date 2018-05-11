using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Crawler.Proxy
{
    public class LiveNodeWatch : IWatcher
    {
        public void Process(WatchedEvent @event)
        {
            if (@event.Path.StartsWith("/live_nodes/"))
            {
                var ip = @event.Path.Replace("/live_nodes/", "");

                if (@event.Type == EventType.NodeCreated)
                {
                    ProxyServer.Instance.UpdateLiveNodes(ip);
                }
            }

            if (pcce.getType() == PathChildrenCacheEvent.Type.CHILD_ADDED)
            {
                ZooProxy.Instance.UpdateLiveNodes(pcce.getData());
            }

            if (pcce.getType() == PathChildrenCacheEvent.Type.CHILD_REMOVED)
            {
                ZooProxy.Instance.UpdateLiveNodes(pcce.getData(), 1);
            }

            if (pcce.getType() == PathChildrenCacheEvent.Type.CHILD_UPDATED)
            {
                ZooProxy.Instance.UpdateLiveNodes(pcce.getData(), 2);
            }
        }
    }
}
