using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public enum ProxyTypeEnum
    {
        Crawler,
        Extracter,
        Feed
    }

    public class LiveProxy
    {
        public ProxyTypeEnum Type { get; set; }

        public string BaseUrl { get; set; }

        public ulong Counts { get; set; }

        public static ProxyTypeEnum GetType(string data)
        {
            if (data.IndexOf("crawler") != -1)
                return ProxyTypeEnum.Crawler;

            if (data.IndexOf("extracter") != -1)
                return ProxyTypeEnum.Extracter;

            return ProxyTypeEnum.Feed;
        }
    }
}