using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils
{
    class Logger
    {
        static ILog log = null;

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger("Logger");
        }

        public static ILog Log
        {
            get
            {
                return log;
            }
        }
    }
}