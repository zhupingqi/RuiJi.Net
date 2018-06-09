using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    public class Logger
    {
        private static Logger _instance;

        /// <summary>
        /// 单例实例
        /// </summary>
        public static Logger Instance { get { return _instance; } }

        /// <summary>
        /// 对象日志静态实体字典
        /// </summary>
        private Dictionary<string, LogModel> Logs { get; set; }

        static Logger()
        {
            _instance = new Logger();
        }

        private Logger()
        {
            Logs = new Dictionary<string, LogModel>();
        }

        public ILog GetLogger(string key)
        { 
            if (!Logs.ContainsKey(key))
                return null;
            return Logs[key].Logger;
        }

        /// <summary>
        /// 添加日志类型
        /// </summary>
        /// <param name="key">节点地址</param>
        /// <param name="types">需要添加的日志追加器集合</param>
        public void Add(string key, List<ILogAppender> logAppenders)
        {
            var repository = LogManager.CreateRepository(key);

            var hasMemory = false;
            var maxMessageCount = 0;
            foreach (var ap in logAppenders)
            {
                var appenders = ap.GetAppender();
                foreach (var appender in appenders)
                {
                    BasicConfigurator.Configure(repository, appender);
                }

                if (ap is MemoryLogAppender)
                {
                    hasMemory = true;
                    maxMessageCount = (ap as MemoryLogAppender).MaxMessageCount;
                }
            }

            if (!Logs.ContainsKey(key))
                Logs.Add(key, new LogModel(LogManager.GetLogger(key, "MyLog"), repository, hasMemory, maxMessageCount));
        }

        /// <summary>
        /// 获取缓存消息
        /// </summary>
        /// <param name="key">节点地址</param>
        /// <param name="start">开始条数</param>
        /// <param name="rows">行数</param>
        /// <returns>消息集合</returns>
        public List<object> GetMessage(string key, int start, int rows)
        {
            if (!Logs.ContainsKey(key))
                return null;

            return Logs[key].Messages.Skip(start).Take(rows).ToList();
        }
    }
}