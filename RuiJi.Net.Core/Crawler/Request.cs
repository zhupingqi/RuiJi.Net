using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class Request
    {
        public string Ip { get; set; }

        public string Cookie { get; set; }

        public NameValueCollection Data { get; set; }

        public bool IsRaw { get; set; }

        public string Charset { get; set; }

        public string Method { get; set; }

        public List<WebHeader> Headers { get; set; }

        public Uri Uri { get; set; }

        public int Timeout { get; set; }

        public string Elect { get; set; }

        public string PostParam { get; set; }

        public bool UseCookie { get; set; }

        public RequestProxy Proxy { get; set; }

        public Request()
        {
            Method = "GET";
            Headers = new List<WebHeader>();
            UseCookie = true;
        }

        public Request(string url,string ip = "") : this()
        {
            this.Uri = new Uri(url);
            this.Ip = ip;
        }
    }
}