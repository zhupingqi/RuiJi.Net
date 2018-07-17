using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;

namespace RuiJi.Net.Node.LTS
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