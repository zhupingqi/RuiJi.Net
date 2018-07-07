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
    /// <summary>
    /// log4net appender base
    /// </summary>
    public abstract class AppenderBase : IAppender
    {
        /// <summary>
        /// log levels
        /// </summary>
        public List<Level> Levels { get; set; }

        /// <summary>
        /// log pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// log layout
        /// </summary>
        public PatternLayout Layout
        {
            get
            {
                return new PatternLayout(Pattern);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public AppenderBase()
        {
            Pattern = GetPattern();

            Levels = new List<Level>() {
                Level.Info,
                Level.Error,
                Level.Fatal
            };
        }

        /// <summary>
        /// configure abstract method
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="repository">repository</param>
        public abstract void Configure(string key, ILoggerRepository repository);

        /// <summary>
        /// get pattern virtual method
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPattern()
        {
            return "%date [%thread] %-5level - %message%newline";
        }
    }
}