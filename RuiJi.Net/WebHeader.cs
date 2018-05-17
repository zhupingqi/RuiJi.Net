using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;

namespace RuiJi.Net
{
    public class WebHeader
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public WebHeader()
        { 
        
        }

        public WebHeader(string key,string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static List<WebHeader> FromWebHeader(WebHeaderCollection headers)
        {
            var hs = new List<WebHeader>();

            foreach (string key in headers.Keys)
            {
                hs.Add(new WebHeader() { 
                   Key = key,
                   Value = headers[key]
                });
            }

            return hs;
        }
    }
}