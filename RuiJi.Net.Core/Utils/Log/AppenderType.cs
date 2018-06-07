using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    /// <summary>
    /// 追加器
    /// </summary>
    public class AppenderType
    {
        public AppenderType(AppenderTypeEnum type, string level, string fileSize = "10MB", LayoutModel layout = null, EamilAppenderModel emailAppender = null, int maxMessageCount = 1000)
        {
            Type = type;
            Level = level;
            FileSize = fileSize;
            Layout = layout == null ? new LayoutModel() : layout;
            EmailAppender = emailAppender;
            MaxMessageCount = maxMessageCount;
        }

        /// <summary>
        /// 追加器类型
        /// </summary>
        public AppenderTypeEnum Type { get; set; }

        /// <summary>
        /// 文件大小（只对文件追加器有效,例*KB|*MB|*GB）
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 日志等级（Email和Memory追加器则为最小等级;例：info,error,fatal）
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志模板（Memory追加器无效）
        /// </summary>
        public LayoutModel Layout { get; set; }

        /// <summary>
        /// 邮件追加器具体内容
        /// </summary>
        public EamilAppenderModel EmailAppender { get; set; }

        /// <summary>
        /// 最大消息数量（只对Memory追加器有效）
        /// </summary>
        public int MaxMessageCount { get; set; }
    }
}
