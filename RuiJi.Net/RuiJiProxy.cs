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
        Extracter
    }

    public class RuiJiProxy
    {
        public ProxyTypeEnum Type { get; set; }

        public string BaseUrl { get; set; }

        public bool Active { get; set; }

        public ulong Counts { get; set; }
    }
}