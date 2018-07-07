using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    /// <summary>
    /// feed request model
    /// </summary>
    public class FeedRequest
    {
        /// <summary>
        /// crawl request
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// crawl setting
        /// </summary>
        public FeedSetting Setting { get; set; }

        /// <summary>
        /// ruiji expression
        /// </summary>
        public string Expression { get; set; }
    }
}