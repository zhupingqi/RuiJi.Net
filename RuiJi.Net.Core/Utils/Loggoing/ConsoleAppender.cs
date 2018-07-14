using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;

namespace RuiJi.Net.Core.Utils.Logging
{
    /// <summary>
    /// log4net console appender
    /// </summary>
    public class ConsoleAppender : AppenderBase
    {
        /// <summary>
        /// configure abstract method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        public override void Configure(string key, ILoggerRepository repository)
        {
            foreach (var level in Levels)
            {
                var appender = new log4net.Appender.ConsoleAppender();

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
