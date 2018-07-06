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
    /// <summary>
    /// crawl response
    /// </summary>
    public class Response
    {
        /// <summary>
        /// response charset
        /// </summary>
        [JsonProperty("charset")]
        public string Charset { get; set; }

        /// <summary>
        /// if true response is raw
        /// </summary>
        [JsonProperty("raw")]
        public bool IsRaw { get; set; }

        /// <summary>
        /// response data
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// response headers
        /// </summary>
        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        /// <summary>
        /// response cookie
        /// </summary>
        [JsonProperty("cookie")]
        public string Cookie { get; set; }

        /// <summary>
        /// response status
        /// </summary>
        [JsonProperty("status")]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// response uri
        /// </summary>
        [JsonProperty("responseUri")]
        public Uri ResponseUri { get; set; }

        /// <summary>
        /// crawl request
        /// </summary>
        [JsonProperty("request")]
        public Request Request { get; set; }

        /// <summary>
        /// response elect info serverip/clientip
        /// </summary>
        [JsonProperty("elect")]
        public string ElectInfo { get; set; }

        /// <summary>
        /// response extensions
        /// </summary>
        [JsonProperty("exts")]
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