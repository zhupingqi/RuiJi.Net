using Newtonsoft.Json;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class Request : ICloneable
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("cookie")]
        public string Cookie { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("elect")]
        public string Elect { get; set; }

        [JsonProperty("useCookie")]
        public bool UseCookie { get; set; }

        [JsonProperty("waitDom")]
        public string WaitDom { get; set; }

        [JsonProperty("proxy")]
        public RequestProxy Proxy { get; set; }

        [JsonProperty("runJs")]
        public bool RunJS { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

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

        public Request(Uri uri, string ip = "") : this()
        {
            this.Uri = uri;
            this.Ip = ip;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}