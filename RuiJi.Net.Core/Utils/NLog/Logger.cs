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

namespace RuiJi.Net.Core.Utils.NLog
{
    public class Logger
    {
        static Logger()
        {
            
        }

        public static bool Add(string key, List<IAppender> appenders)
        {
            try
            {
                ILoggerRepository repository;

                var log = LogManager.Exists(key);
                if (log == null)
                    repository = LogManager.CreateRepository(key);
                else
                    repository = log.Logger.Repository;

                foreach (var appender in appenders)
                {
                    appender.Configure(key, repository);
                }
                
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static ILog GetLogger(string key)
        {
            return LogManager.GetLogger(key, "ruiji.net");
        }
    }
}