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

namespace RuiJi.Net.Core.Utils.NLog
{
    public class MemoryAppender : AppenderBase
    {
        public int MaxMessage { get; set; }

        public log4net.Appender.MemoryAppender memoryAppender { get; private set; }

        private Thread watcher;

        private bool watchMessage;

        private string key;

        private static Dictionary<string,List<string>> Messages;

        private static object _lck = new object();

        static MemoryAppender()
        {
            Messages = new Dictionary<string, List<string>>();
        }

        public MemoryAppender(int maxMessage = 1000)
        {
            this.MaxMessage = maxMessage;
            
            Levels = new List<log4net.Core.Level>
            {
                Level.Fatal
            };
        }

        public override void Configure(string key, ILoggerRepository repository)
        {
            this.key = key;

            var appender = new log4net.Appender.MemoryAppender();
            appender.Name = "MemoryAppender";
            appender.Threshold = Levels[0];

            var layout = new PatternLayout(Pattern);
            layout.ActivateOptions();
            appender.Layout = layout;

            appender.ActivateOptions();

            BasicConfigurator.Configure(repository, appender);

            memoryAppender = appender;

            Start();
        }

        public void Start()
        {
            Messages.Add(key, new List<string>());
            watchMessage = true;

            watcher = new Thread(()=> {
                Watch(key);
            });

            watcher.Start();
        }

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

        public static string[] GetMessage(string key)
        {
            lock (_lck)
            {
                var msgs = Messages[key].ToArray();

                Messages[key].Clear();

                return msgs;
            }
        }

        private void Watch(string key)
        {
            while (watchMessage)
            {
                var events = memoryAppender.GetEvents();
                if (events != null && events.Length > 0)
                {
                    lock (_lck)
                    {
                        foreach (var ev in events)
                        {
                            var layout = new PatternLayout(Pattern);
                            layout.ActivateOptions();
                            var w = new StringWriter();
                            layout.Format(w, ev);

                            var msg = w.GetStringBuilder().ToString();
                            Messages[key].Insert(0, msg);

                            while (Messages[key].Count > MaxMessage)
                            {
                                Messages[key].RemoveAt(Messages.Count - 1);
                            }
                        }
                    }
                    memoryAppender.Clear();
                }

                Thread.Sleep(5000);
            }
        }
    }
}