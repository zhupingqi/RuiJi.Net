using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    public interface ILogAppender
    {
        List<Level> Levels { get; set; }

        LayoutModel Layout { get; set; }

        List<IAppender> GetAppender();
    }
}
