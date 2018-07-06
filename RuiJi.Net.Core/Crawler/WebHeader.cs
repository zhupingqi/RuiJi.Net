using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;

namespace RuiJi.Net.Core.Crawler
{
    /// <summary>
    /// web headers used by crawl request and crawl response
    /// </summary>
    public class WebHeader
    {
        /// <summary>
        /// header name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// header value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public WebHeader()
        { 
        
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="key">header key</param>
        /// <param name="value">header value</param>
        public WebHeader(string key,string value)
        {
            this.Name = key;
            this.Value = value;
        }

        /// <summary>
        /// convert web header collection to webheader list
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static List<WebHeader> FromWebHeader(WebHeaderCollection headers)
        {
            var hs = new List<WebHeader>();

            foreach (string key in headers.Keys)
            {
                hs.Add(new WebHeader() { 
                   Name = key,
                   Value = headers[key]
                });
            }

            return hs;
        }
    }
}