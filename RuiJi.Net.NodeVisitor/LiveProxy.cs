using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public enum NodeProxyTypeEnum
    {
        CRAWLERPROXY = 0,
        EXTRACTERPROXY = 1,
        FEEDPROXY = 2
    }

    public class LiveProxy
    {
        public NodeProxyTypeEnum Type { get; set; }

        public string BaseUrl { get; set; }

        public ulong Counts { get; set; }

        public static NodeProxyTypeEnum GetType(string data)
        {
            if (data.IndexOf("crawler") != -1)
                return NodeProxyTypeEnum.CRAWLERPROXY;

            if (data.IndexOf("extracter") != -1)
                return NodeProxyTypeEnum.EXTRACTERPROXY;

            return NodeProxyTypeEnum.FEEDPROXY;
        }
    }
}