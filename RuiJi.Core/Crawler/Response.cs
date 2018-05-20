using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Core.Crawler
{
    public class Response
    {
        public string Charset { get; set; }

        public string Method { get; set; }

        public bool IsRaw { get; set; }

        public object Data { get; set; }

        public List<WebHeader> Headers { get; set; }

        public string Cookie { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Uri ResponseUri { get; set; }

        public Uri RequestUri { get; set; }

        public string ElectInfo { get; set; }
    }
}