using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node.Feed.LTS;

namespace RuiJi.Net.Node
{
    public class StandaloneNode : INode
    {
        private FeedScheduler scheduler;

        static StandaloneNode()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public string BaseUrl { get; protected set; }

        public NodeTypeEnum NodeType { get; protected set; }

        public DateTime StartTime { get; protected set; }

        public bool IsLeader { get; protected set; }

        public StandaloneNode(string baseUrl)
        {
            BaseUrl = baseUrl;
            NodeType = NodeTypeEnum.STANDALONE;

            StartTime = DateTime.Now;
        }

        public NodeData GetData(string path)
        {
            return new NodeData();
        }

        public void SetData(string path, string data, int version = -1)
        {

        }

        public void Start()
        {
            Logger.Add(BaseUrl, new List<IAppender> {
                new RollingFileAppender(BaseUrl),
                new MemoryAppender(),
                new ConsoleAppender()
            });

            scheduler = new FeedScheduler();
            scheduler.Start(BaseUrl, null);

            Logger.GetLogger(BaseUrl).Info("Start WebApiServer At http://" + BaseUrl + " with " + NodeType.ToString() + " node");
        }

        public void Stop()
        {
            if (scheduler != null)
            {
                scheduler.Stop();
                scheduler = null;
            }
        }
    }
}
