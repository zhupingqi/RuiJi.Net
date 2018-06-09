using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    /// <summary>
    /// 静态日志实体
    /// </summary>
    public class LogModel
    {
        private Thread watcher;
        public LogModel() { }

        public LogModel(ILog logger, ILoggerRepository repository, bool watchMessage, int maxMessageCount = 1000)
        {
            Logger = logger;
            Repository = repository;
            Messages = new List<object>();
            WatchMessage = watchMessage;
            MaxMessageCount = maxMessageCount;
            watcher = new Thread(new ThreadStart(Watch));
            watcher.Start();
        }
        ~LogModel()
        {
            watcher.Abort();
        }
        public ILog Logger { get; set; }

        private ILoggerRepository Repository { get; set; }

        public List<object> Messages { get; private set; }

        /// <summary>
        /// 是否启用监察缓存消息
        /// </summary>
        private bool WatchMessage { get; set; }

        /// <summary>
        /// 单对象日志消息最大缓存数量
        /// </summary>
        private int MaxMessageCount { get; set; }

        /// <summary>
        /// 监察缓存消息
        /// </summary>
        private void Watch()
        {
            while (WatchMessage)
            {
                var appenders = Repository.GetAppenders();
                foreach (var appender in appenders)
                {
                    if (appender is MemoryAppender)
                    {
                        var ap = ((MemoryAppender)appender);
                        var events = ap.GetEvents();
                        if (events != null && events.Length > 0)
                        {
                            foreach (var ev in events)
                            {
                                var msgObj = new { level = ev.Level.ToString().ToLower(), thread = ev.ThreadName, time = ev.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"), message = ev.RenderedMessage };
                                lock (this)
                                {
                                    while (Messages.Count >= MaxMessageCount)
                                    {
                                        Messages.RemoveAt(Messages.Count - 1);
                                    }
                                    Messages.Insert(0, msgObj);
                                }
                            }
                            ap.Clear();
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }
}
