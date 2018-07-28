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

namespace RuiJi.Net.Core.Utils.Logging
{
    /// <summary>
    /// logger used by ruiji.net
    /// </summary>
    public class Logger
    {
        static Dictionary<string, ILoggerRepository> logger = new Dictionary<string, ILoggerRepository>();

        static Logger()
        {            
        }

        /// <summary>
        /// add appender
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="appenders">appenders</param>
        /// <returns></returns>
        public static bool Add(string key, List<IAppender> appenders)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    key = "unknown";

                ILoggerRepository repository;

                if (!logger.ContainsKey(key))
                {
                    repository = LogManager.CreateRepository(key);
                    logger.Add(key, repository);
                }
                else
                {
                    repository = logger[key];
                }                    

                foreach (var appender in appenders)
                {
                    appender.Configure(key, repository);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// get logger
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>ILog interface</returns>
        public static ILog GetLogger(string key)
        {
            if (string.IsNullOrEmpty(key))
                key = "unknown";

            if (!logger.ContainsKey(key))
            {
                Add(key, new List<IAppender> {
                     new RollingFileAppender("")
                });
            }

            return LogManager.GetLogger(key, "ruiji.net");
        }
    }
}