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
        static Dictionary<string, ILoggerRepository> logger = new Dictionary<string, ILoggerRepository>();

        static Logger()
        {
            
        }

        public static bool Add(string key, List<IAppender> appenders)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    key = "unknown";

                ILoggerRepository repository;

                //var log = logger.ContainsKey(key);
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
            catch (Exception ex)
            {
                return false;
            }
        }

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