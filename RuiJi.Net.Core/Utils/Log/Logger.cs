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

        public LogModel GetLogger(string baseUrl, string nodetype)
        {
            string path = nodetype + "/" + baseUrl.Replace(".", "_").Replace(":", "_");
            if (!Logs.ContainsKey(path))
                return null;
            return Logs[path];
        }

        /// <summary>
        /// 添加日志类型
        /// </summary>
        /// <param name="baseUrl">节点地址</param>
        /// <param name="nodetype">节点类型</param>
        /// <param name="types">需要添加的日志追加器集合</param>
        public void AddNewLogger(string baseUrl, string nodetype, List<AppenderType> types)
        {
            string path = nodetype + "/" + baseUrl.Replace(".", "_").Replace(":", "_");
            var repository = LogManager.CreateRepository(path);
            var hasMemory = false;
            var maxMessageCount = 0;
            foreach (var type in types)
            {
                var level = typeof(Level).GetField(System.Text.RegularExpressions.Regex.Replace(type.Level.ToLower(), @"^\w", t => t.Value.ToUpper())).GetValue(null) as Level;
                switch (type.Type)
                {
                    case AppenderTypeEnum.INFO:
                    case AppenderTypeEnum.ERROR:
                    case AppenderTypeEnum.FATAL:
                        BasicConfigurator.Configure(repository, GetFileAppender(path, level, type.Layout, type.FileSize));
                        break;
                    case AppenderTypeEnum.MESSAGE:
                        BasicConfigurator.Configure(repository, GetMemoryAppender(level));
                        hasMemory = true;
                        maxMessageCount = type.MaxMessageCount;
                        break;
                    case AppenderTypeEnum.EMAIL:
                        if (type.EmailAppender != null)
                        {
                            BasicConfigurator.Configure(repository, GetSmtpAppender(type.EmailAppender.To, type.EmailAppender.From, type.EmailAppender.Password, type.EmailAppender.Subject, type.EmailAppender.Host, type.Layout, level));
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!Logs.ContainsKey(path))
                Logs.Add(path, new LogModel(LogManager.GetLogger(path, "MyLog"), repository, hasMemory, maxMessageCount));
        }

        /// <summary>
        /// 获取缓存消息
        /// </summary>
        /// <param name="baseUrl">节点地址</param>
        /// <param name="nodetype">节点类型</param>
        /// <param name="start">开始条数</param>
        /// <param name="rows">行数</param>
        /// <returns>消息集合</returns>
        public List<object> GetMessage(string baseUrl, string nodetype, int start, int rows)
        {
            string path = nodetype + "/" + baseUrl.Replace(".", "_").Replace(":", "_");
            if (!Logs.ContainsKey(path))
                return null;

            return Logs[path].Messages.Skip(start).Take(rows).ToList();
        }

        /// <summary>
        /// 获取等级范围筛选器
        /// </summary>
        /// <param name="maxLevel">最大级别（可与最小级别相同）</param>
        /// <param name="minLevel">最小级别（可与最大级别相同）</param>
        /// <returns></returns>
        private LevelRangeFilter GetLevelFilter(Level maxLevel, Level minLevel)
        {
            var filter = new LevelRangeFilter();
            filter.LevelMax = maxLevel;
            filter.LevelMin = minLevel;
            filter.ActivateOptions();
            return filter;
        }

        /// <summary>
        /// 获取日志格式化布局
        /// </summary>
        /// <param name="container">正文</param>
        /// <param name="hearder">头部</param>
        /// <param name="footer">尾部</param>
        /// <returns></returns>
        private PatternLayout GetLayout(string container, string hearder, string footer)
        {
            var layout = new log4net.Layout.PatternLayout(container);
            layout.Header = hearder + Environment.NewLine;
            layout.Footer = footer + Environment.NewLine;
            return layout;
        }

        /// <summary>
        /// 获取滚动文件追加器
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="level">等级</param>
        /// <param name="fileSize">文件大小(?KB|?MB|?GB)</param>
        /// <returns>滚动文件追加器</returns>
        private RollingFileAppender GetFileAppender(string path, Level level, LayoutModel layout, string fileSize = "10MB")
        {
            var appender = new RollingFileAppender();

            appender.AppendToFile = true;
            appender.File = "logs/" + path + "/" + level.ToString().ToLower() + ".log";
            appender.ImmediateFlush = true;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.Name = level.ToString().ToLower() + "Appender";
            appender.MaximumFileSize = fileSize;
            appender.Layout = GetLayout(layout.Container, layout.Header, layout.Footer);
            appender.AddFilter(GetLevelFilter(level, level));
            appender.ActivateOptions();
            return appender;

        }

        /// <summary>
        /// 获取邮件发送错误日志追加器(qq邮箱ssl加密，必须使用最新版log4net，将EnableSsl设为true)
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="from">发件人</param>
        /// <param name="password">密码</param>
        /// <param name="subject">主题</param>
        /// <param name="host">主机</param>
        /// <param name="minLevel">最小级别</param>
        private SmtpAppender GetSmtpAppender(string to, string from, string password, string subject, string host, LayoutModel layout, Level minLevel = null)
        {

            var appender = new SmtpAppender();
            appender.Authentication = SmtpAppender.SmtpAuthentication.Basic;
            appender.Name = "SmtpAppender";

            appender.To = to;
            appender.From = from;
            appender.Username = from;
            appender.Password = password;
            appender.Subject = subject;
            appender.SmtpHost = host;

            appender.Lossy = true;

            minLevel = minLevel == null ? Level.Fatal : minLevel;
            appender.Threshold = minLevel;
            appender.Evaluator = new log4net.Core.LevelEvaluator(minLevel);

            appender.Layout = GetLayout(layout.Container, layout.Header, layout.Footer);
            appender.ActivateOptions();
            return appender;

        }

        /// <summary>
        /// 获取缓存区日志追加器
        /// </summary>
        /// <returns>缓存区日志追加器</returns>
        private MemoryAppender GetMemoryAppender(Level level)
        {
            var appender = new MemoryAppender();
            appender.Name = "MemoryAppender";
            appender.Threshold = level;
            appender.ActivateOptions();
            return appender;
        }
    }
}