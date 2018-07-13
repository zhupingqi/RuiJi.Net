using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public enum NodeTypeEnum
    {
        CRAWLERPROXY = 0,
        EXTRACTORPROXY = 1,
        FEEDPROXY = 2,
        CRAWLER = 3,
        EXTRACTOR = 4,
        FEED = 5,
        STANDALONE
    }
}