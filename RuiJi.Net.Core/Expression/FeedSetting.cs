using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    /// <summary>
    /// expression feedsetting model
    /// </summary>
    public class FeedSetting: IRuiJiParseResult
    {
        /// <summary>
        /// corn expression
        /// </summary>
        public string CornExpression { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// after a few minutes of delay, start to grab the update link
        /// </summary>
        public int Delay { get; set; }
    }
}