using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Configuration
{
    public class Node
    {
        /// <summary>
        /// node url
        /// </summary>
        [JsonProperty("baseUrl")]
        public string BaseUrl
        {
            get;
            set;
        }

        /// <summary>
        /// node type
        /// </summary>
        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// node proxy url
        /// </summary>
        [JsonProperty("proxy")]
        public string Proxy
        {
            get;
            set;
        }
    }
}
