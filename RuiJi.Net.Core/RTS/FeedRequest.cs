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
    public class FeedRequest
    {
        public Request Request { get; set; }

        public FeedSetting Setting { get; set; }

        public string Expression { get; set; }
    }
}