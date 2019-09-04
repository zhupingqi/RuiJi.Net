using Newtonsoft.Json;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Node.LTS;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Node.Feed.Db
{
    public enum FeedTypeEnum
    {
        HTML,
        XML,
        JS,
        JSON,
        JSONP
    }

    public class FeedModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sitename")]
        public string SiteName { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConvert<FeedTypeEnum>))]
        public FeedTypeEnum Type { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("ua")]
        public string UA { get; set; }

        [JsonProperty("headers")]
        public string Headers { get; set; }

        [JsonProperty("scheduling")]
        public string Scheduling { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(EnumConvert<Status>))]
        public Status Status { get; set; }

        [JsonProperty("runJs")]
        [JsonConverter(typeof(EnumConvert<Status>))]
        public Status RunJS { get; set; }

        [JsonProperty("waitDom")]
        public string WaitDom { get; set; }

        [JsonProperty("feedonly")]
        public bool FeedOnly { get; set; }

        [JsonProperty("block")]
        public string BlockExpression { get; set; }

        [JsonProperty("rexp")]
        public string RuiJiExpression { get; set; }

        public static FeedRequest ToFeedRequest(FeedModel feed)
        {
            var request = new Request(feed.Address);
            request.RunJS = (feed.RunJS == Status.ON);
            if (request.RunJS)
                request.WaitDom = feed.WaitDom;
            
            if (feed.Headers != null)
            {
                request.Headers = GetHeaders(feed.Headers);

                if (request.Headers.Count(m => m.Name == "Referer") == 0)
                    request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
            }

            request.Method = feed.Method;
            if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.Data))
            {
                request.ContentType = feed.ContentType;
                request.Data = feed.Data;
            }

            var ua = string.IsNullOrEmpty(feed.UA) ? NodeVisitor.Setter.GetRandomSettingUA() : feed.UA;
            if (!string.IsNullOrEmpty(ua))
                request.Headers.Add(new WebHeader("User-Agent", ua));

            return new FeedRequest
            {
                Request = request,
                Setting = new Core.Expression.FeedSetting
                {
                    Id = feed.Id.ToString(),
                    Delay = feed.Delay
                },
                Expression = feed.RuiJiExpression
            };
        }

        private static List<WebHeader> GetHeaders(string headers)
        {
            var result = new List<WebHeader>();
            if (string.IsNullOrEmpty(headers))
                return result;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(headers)))
            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var sp = line.Split(':');

                    if (sp.Length < 2)
                    {
                        /*
                         * 这里原来的 continue 会导致死循环 endless loop:
                         * Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36
                         */
                        goto next;
                    }

                    var endIndex = line.IndexOf(':');
                    result.Add(new WebHeader(line.Substring(0, endIndex),
                        line.Substring(endIndex + 1)));

                    if (reader.EndOfStream)
                        break;

                    next:
                    line = reader.ReadLine();
                }
            }

            return result;
        }
    }
}