using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    /// <summary>
    /// crawler node elect result
    /// </summary>
    public class CrawlerElectResult
    {
        /// <summary>
        /// crawler node base url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// crawler node ip
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// elected proxy
        /// </summary>
        public RequestProxy Proxy { get; set; }
    }
}