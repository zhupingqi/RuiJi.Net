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

        private List<string> Messages;

        public MemoryAppender(int maxMessage = 1000)
        {
            this.MaxMessage = maxMessage;
            Messages = new List<string>();
            Levels = new List<log4net.Core.Level>
            {
                Level.Fatal
            };
        }

        public override void Configure(string key, ILoggerRepository repository)
        {
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
            watchMessage = true;

            watcher = new Thread(new ThreadStart(Watch));
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
        }

        public string[] GetMessage()
        {
            lock (this)
            {
                var msgs = Messages.ToArray();

                Messages.Clear();

                return msgs;
            }
        }

        private void Watch()
        {
            while (watchMessage)
            {
                var events = memoryAppender.GetEvents();
                if (events != null && events.Length > 0)
                {
                    lock (this)
                    {
                        foreach (var ev in events)
                        {
                            var layout = new PatternLayout(Pattern);
                            layout.ActivateOptions();
                            var t = new StringWriter();
                            layout.Format(t, ev);

                            var msgObj = t.GetStringBuilder().ToString();

                            while (Messages.Count > MaxMessage)
                            {
                                Messages.RemoveAt(Messages.Count - 1);
                            }

                            Messages.Insert(0, msgObj);
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}