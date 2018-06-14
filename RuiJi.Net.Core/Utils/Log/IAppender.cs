using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    public interface IAppender
    {
        List<Level> Levels { get; set; }

        string Pattern { get; set; }

        void Configure(string key, ILoggerRepository repository);
    }
}