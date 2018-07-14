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

namespace RuiJi.Net.Core.Utils.Logging
{
    /// <summary>
    /// rolling file logger
    /// </summary>
    public class RollingFileAppender : AppenderBase
    {
        /// <summary>
        /// logger sub path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// logger folder
        /// </summary>
        public static string Folder = "logs/";

        /// <summary>
        /// file size
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="path">logger sub path</param>
        /// <param name="filesize">filesize</param>
        public RollingFileAppender(string path ,string filesize = "10M")
        {
            this.Path = path;
            this.FileSize = filesize;
        }

        /// <summary>
        /// configure method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        public override void Configure(string key, ILoggerRepository repository)
        {
            var path = Path.Replace(":", "_");

            foreach (var level in Levels)
            {
                var appender = new log4net.Appender.RollingFileAppender();
                appender.File = Folder + (string.IsNullOrEmpty(path) ? "" : path) + "/" + level.ToString().ToLower() + ".log";
                appender.AppendToFile = true;
                appender.ImmediateFlush = true;
                appender.LockingModel = new FileAppender.MinimalLock();
                appender.Threshold = level;

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
