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
    public abstract class AppenderBase : IAppender
    {
        public List<Level> Levels { get; set; }

        public string Pattern { get; set; }

        public PatternLayout Layout
        {
            get
            {
                return new PatternLayout(Pattern);
            }
        }

        public AppenderBase()
        {
            Pattern = GetPattern();

            Levels = new List<Level>() {
                Level.Info,
                Level.Error,
                Level.Fatal
            };
        }

        public abstract void Configure(string key, ILoggerRepository repository);

        protected virtual string GetPattern()
        {
            return "%date [%thread] %-5level - %message%newline";
        }
    }
}