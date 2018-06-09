using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace RuiJi.Net.Core.Utils.Log
{
    /// <summary>
    /// 追加器
    /// </summary>
    public abstract class LogAppenderBase : ILogAppender
    {
        /// <summary>
        /// 日志等级（Email和Memory追加器则为最小等级;例：info,error,fatal）
        /// </summary>
        public List<Level> Levels { get; set; }

        /// <summary>
        /// 日志模板（Memory追加器无效）
        /// </summary>
        public LayoutModel Layout { get; set; }

        public abstract List<IAppender> GetAppender();

        public LogAppenderBase()
        {
            Levels = null;
            Layout = new LayoutModel();
        }

        public LogAppenderBase(List<Level> levels)
        {
            Levels = levels;
            Layout = new LayoutModel();
        }


        public LogAppenderBase(LayoutModel layout)
        {
            Levels = null;
            Layout = layout;
        }

        public LogAppenderBase(List<Level> levels, LayoutModel layout)
        {
            Levels = levels;
            Layout = layout;
        }


        /// <summary>
        /// 获取等级范围筛选器
        /// </summary>
        /// <param name="maxLevel">最大级别（可与最小级别相同）</param>
        /// <param name="minLevel">最小级别（可与最大级别相同）</param>
        /// <returns></returns>
        protected LevelRangeFilter GetLevelFilter(Level maxLevel, Level minLevel)
        {
            var filter = new LevelRangeFilter();
            filter.LevelMax = maxLevel;
            filter.LevelMin = minLevel;
            filter.ActivateOptions();
            return filter;
        }

        /// <summary>
        /// 获取日志格式化布局
        /// </summary>
        /// <param name="container">正文</param>
        /// <param name="hearder">头部</param>
        /// <param name="footer">尾部</param>
        /// <returns></returns>
        protected PatternLayout GetLayout(string container, string hearder, string footer)
        {
            var layout = new PatternLayout(container);
            layout.Header = hearder + Environment.NewLine;
            layout.Footer = footer + Environment.NewLine;
            return layout;
        }
    }
}
