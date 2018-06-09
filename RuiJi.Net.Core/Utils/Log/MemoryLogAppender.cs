using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace RuiJi.Net.Core.Utils.Log
{
    public class MemoryLogAppender : LogAppenderBase
    {

        public MemoryLogAppender(int maxMessageCount = 1000) : base()
        {
            MaxMessageCount = maxMessageCount;
        }

        public MemoryLogAppender(LayoutModel layout, int maxMessageCount = 1000) : base(layout)
        {
            MaxMessageCount = maxMessageCount;
        }

        public MemoryLogAppender(List<Level> levels, int maxMessageCount = 1000) : base(levels)
        {
            MaxMessageCount = maxMessageCount;
        }
        public MemoryLogAppender(List<Level> levels, LayoutModel layout, int maxMessageCount = 1000) : base(levels, layout)
        {
            MaxMessageCount = maxMessageCount;
        }

        /// <summary>
        /// 最大消息数量
        /// </summary>
        public int MaxMessageCount { get; set; }

        public override List<IAppender> GetAppender()
        {
            var level = Levels == null || Levels.Count == 0 ? log4net.Core.Level.All : Levels[0];
            var appender = new MemoryAppender();
            appender.Name = "MemoryAppender";
            appender.Threshold = level;
            appender.ActivateOptions();
            return new List<IAppender>() { appender };
        }
    }
}
