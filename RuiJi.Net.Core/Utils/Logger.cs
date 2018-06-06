using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    public class Logger
    {
        private static Logger _instance;

        public static Logger Instance { get { return _instance; } }

        public const int MAXMESSAGECOUNT = 100;

        public Dictionary<string, List<object>> Messages { get; private set; }

        public Dictionary<string, ILog> Logs { get; private set; }

        private Logger()
        {
            Logs = new Dictionary<string, ILog>();
            Messages = new Dictionary<string, List<object>>();
        }

        private LevelRangeFilter GetLevelFilter(Level level)
        {
            var filter = new LevelRangeFilter();
            filter.LevelMax = level;
            filter.LevelMin = level;
            filter.ActivateOptions();
            return filter;
        }

        private PatternLayout GetLayout(string container = "%date [%thread] %-5level - %message%newline", string hearder = "------ New session ------", string footer = "------ End session ------")
        {
            var layout = new log4net.Layout.PatternLayout(container);
            layout.Header = hearder;
            layout.Footer = footer;
            return layout;
        }

        private RollingFileAppender GetFileAppender(string path, Level level)
        {
            var appender = new RollingFileAppender();

            appender.AppendToFile = true;
            appender.File = "logs/" + path + "/" + level.ToString().ToLower() + ".log";
            appender.ImmediateFlush = true;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.Name = level.ToString().ToLower() + "Appender";
            appender.MaximumFileSize = "256MB";
            appender.Layout = GetLayout();
            appender.AddFilter(GetLevelFilter(level));
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
        private SmtpAppender GetSmtpAppender(string to, string from, string password, string subject, string host, Level minLevel = null)
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

            appender.Layout = GetLayout();
            appender.ActivateOptions();
            return appender;

        }

        private MemoryAppender GetMemoryAppender()
        {
            var appender = new MemoryAppender();
            appender.Name = "MemoryAppender";
            appender.Threshold = Level.All;
            appender.ActivateOptions();
            return appender;
        }
    }
}