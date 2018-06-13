using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;

namespace RuiJi.Net.Core.Utils.NLog
{
    public class RollingFileAppender : AppenderBase
    {
        public string Path { get; set; }

        public string FileSize { get; set; }

        public RollingFileAppender(string path ,string filesize = "10M")
        {
            this.Path = path;
            this.FileSize = filesize;
        }

        public override void Configure(string key, ILoggerRepository repository)
        {
            var path = Path.Replace(":", "_");

            foreach (var level in Levels)
            {
                var appender = new log4net.Appender.RollingFileAppender();
                appender.File = path + "/" + level.ToString().ToLower() + ".log";
                appender.AppendToFile = true;
                appender.ImmediateFlush = true;
                appender.LockingModel = new FileAppender.MinimalLock();

                var layout = new PatternLayout(Pattern);
                layout.ActivateOptions();

                appender.Layout = layout;
                appender.Name = key + "_" + level.ToString().ToLower();

                var filter = new LevelRangeFilter();
                filter.LevelMax = level;
                filter.LevelMin = level;
                filter.ActivateOptions();
                appender.AddFilter(filter);

                appender.ActivateOptions();

                BasicConfigurator.Configure(repository, appender);
            }
        }
    }
}
