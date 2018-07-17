using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Logging
{
    /// <summary>
    /// memory log appender
    /// </summary>
    public class MemoryAppender : AppenderBase
    {
        /// <summary>
        /// max message length
        /// </summary>
        public int MaxMessage { get; set; }

        private Thread watcher;

        private bool watchMessage;

        private string key;

        private static Dictionary<string, List<string>> Messages;

        private static object _lck = new object();

        static MemoryAppender()
        {
            Messages = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="maxMessage">max message length</param>
        public MemoryAppender(int maxMessage = 1000)
        {
            this.MaxMessage = maxMessage;
        }

        /// <summary>
        /// configure abstract method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        public override void Configure(string key, ILoggerRepository repository)
        {
            this.key = key;
            var appenders = new List<log4net.Appender.MemoryAppender>();

            foreach (var level in Levels)
            {
                var appender = new log4net.Appender.MemoryAppender();
                appender.Name = "MemoryAppender";
                appender.Threshold = level;

                var layout = new PatternLayout(Pattern);
                layout.ActivateOptions();
                appender.Layout = layout;

                appender.ActivateOptions();

                BasicConfigurator.Configure(repository, appender);

                appenders.Add(appender);
            }

            Start(key, appenders);
        }

        /// <summary>
        /// start memory logger
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="appenders">memory appenders</param>
        public void Start(string key, List<log4net.Appender.MemoryAppender> appenders)
        {
            Messages.Add(key, new List<string>());
            watchMessage = true;

            watcher = new Thread(() =>
            {
                Watch(key, appenders);
            });

            watcher.Start();
        }

        /// <summary>
        /// stop memory logger
        /// </summary>
        public void Stop()
        {
            watchMessage = false;

            if (watcher != null)
            {
                watcher.Abort();
                watcher = null;
            }

            Messages.Remove(key);
        }

        /// <summary>
        /// get memory logger message
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>messages</returns>
        public static string[] GetMessage(string key)
        {
            lock (_lck)
            {
                if (Messages.ContainsKey(key))
                {
                    var msgs = Messages[key].ToArray();

                    Messages[key].Clear();

                    return msgs;
                }

                return new string[0];
            }
        }

        private void Watch(string key, List<log4net.Appender.MemoryAppender> appenders)
        {
            while (watchMessage)
            {
                lock (_lck)
                {
                    var events = new List<LoggingEvent>();

                    foreach (var appender in appenders)
                    {
                        var evs = appender.GetEvents();
                        if (evs != null && evs.Length > 0)
                        {
                            events.AddRange(evs);
                            appender.Clear();
                        }
                    }

                    if (events.Count > 0)
                    {
                        foreach (var ev in events)
                        {
                            var layout = new PatternLayout(Pattern);
                            layout.ActivateOptions();
                            var w = new StringWriter();
                            layout.Format(w, ev);

                            var msg = w.GetStringBuilder().ToString();
                            Messages[key].Add(msg);

                            while (Messages[key].Count > MaxMessage)
                            {
                                Messages[key].RemoveAt(0);
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}