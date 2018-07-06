using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
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
    /// <summary>
    /// crawl request
    /// </summary>
    public class Request : ICloneable, IRuiJiParseResult
    {
        /// <summary>
        /// crawl used ip
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// crawl used cookie
        /// </summary>
        [JsonProperty("cookie")]
        public string Cookie { get; set; }

        /// <summary>
        /// post data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// charset
        /// </summary>
        [JsonProperty("charset")]
        public string Charset { get; set; }

        /// <summary>
        /// crawl method GET/POST
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// crawl headers
        /// </summary>
        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        /// <summary>
        /// crawl content type
        /// </summary>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// crawl uri
        /// </summary>
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        /// <summary>
        /// crawl timeout
        /// </summary>
        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        /// <summary>
        /// crawl elected node server baseUrl
        /// </summary>
        [JsonProperty("elect")]
        public string Elect { get; set; }

        /// <summary>
        /// if true use cookie
        /// </summary>
        [JsonProperty("useCookie")]
        public bool UseCookie { get; set; }

        /// <summary>
        /// wait dom(jquery selector) util dom ready when runJs is true
        /// </summary>
        [JsonProperty("waitDom")]
        public string WaitDom { get; set; }

        /// <summary>
        /// crawl with proxy
        /// </summary>
        [JsonProperty("proxy")]
        public RequestProxy Proxy { get; set; }

        /// <summary>
        /// if true crawler will use PhantomCrawler
        /// </summary>
        [JsonProperty("runJs")]
        public bool RunJS { get; set; }

        /// <summary>
        /// http username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// http password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// tag to record you need
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Request()
        {
            Method = "GET";
            Headers = new List<WebHeader>();
            UseCookie = true;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="ip">use ip address</param>
        public Request(string url,string ip = "") : this()
        {
            this.Uri = new Uri(url);
            this.Ip = ip;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="ip">use ip address</param>
        public Request(Uri uri, string ip = "") : this()
        {
            this.Uri = uri;
            this.Ip = ip;
        }

        /// <summary>
        /// clone
        /// </summary>
        /// <returns>new request object</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}