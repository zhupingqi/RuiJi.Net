using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Logging
{
    public interface IAppender
    {
        /// <summary>
        /// log levels
        /// </summary>
        List<Level> Levels { get; set; }

        /// <summary>
        /// log pattern
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// configure abstract method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        void Configure(string key, ILoggerRepository repository);
    }
}