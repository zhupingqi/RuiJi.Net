using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace RuiJi.Net.Core.Utils.Log
{
    public class RollingFileLogAppender : LogAppenderBase
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="fileSize">存储文件最大值（*:KB|*:MB|*:GB）</param>
        public RollingFileLogAppender(string path, string fileSize = "10MB") : base()
        {
            Path = path;
            FileSize = fileSize;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="fileSize">存储文件最大值（*:KB|*:MB|*:GB）</param>
        public RollingFileLogAppender(LayoutModel layout, string path, string fileSize = "10MB") : base(layout)
        {
            Path = path;
            FileSize = fileSize;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="fileSize">存储文件最大值（*:KB|*:MB|*:GB）</param>
        public RollingFileLogAppender(List<Level> levels, string path, string fileSize = "10MB") : base(levels)
        {
            Path = path;
            FileSize = fileSize;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="fileSize">存储文件最大值（*:KB|*:MB|*:GB）</param>
        public RollingFileLogAppender(List<Level> levels, LayoutModel layout, string path, string fileSize = "10MB") : base(levels, layout)
        {
            Path = path;
            FileSize = fileSize;
        }

        public string Path { get; set; }

        /// <summary>
        /// 文件大小（只对文件追加器有效,例*KB|*MB|*GB）
        /// </summary>
        public string FileSize { get; set; }

        public override List<IAppender> GetAppender()
        {
            var result = new List<IAppender>();
            Levels = Levels == null || Levels.Count == 0 ? new List<Level>() { Level.Info, Level.Error, Level.Fatal } : Levels;

            var path = Path.Replace(":","_");

            foreach (var level in Levels)
            {
                var appender = new RollingFileAppender();

                appender.AppendToFile = true;
                appender.File = "logs/" + path + "/" + level.ToString().ToLower() + ".log";
                appender.ImmediateFlush = true;
                appender.LockingModel = new FileAppender.MinimalLock();
                appender.Name = level.ToString().ToLower() + "Appender";
                appender.MaximumFileSize = FileSize;
                appender.Layout = GetLayout(Layout.Container, Layout.Header, Layout.Footer);
                appender.AddFilter(GetLevelFilter(level, level));
                appender.ActivateOptions();
            }

            return result;
        }
    }
}
