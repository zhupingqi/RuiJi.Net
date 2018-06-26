using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Utils;

namespace RuiJi.Net.Core.Crawler
{
    public class Response
    {
        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("raw")]
        public bool IsRaw { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        [JsonProperty("cookie")]
        public string Cookie { get; set; }

        [JsonProperty("status")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty("responseUri")]
        public Uri ResponseUri { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("elect")]
        public string ElectInfo { get; set; }

        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonIgnore]
        public string[] Extensions
        {
            get
            {
                if (Headers.Count(m => m.Name == "Content-Type") == 0)
                    return new string[0];

                return Mimes.Extension(Headers.First(m => m.Name == "Content-Type").Value).ToArray();
            }
        }
    }
}