using Newtonsoft.Json;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RuiJi.Net.Core.RTS
{
    /// <summary>
    /// feed monitor job
    /// </summary>
    public class FeedJob : FeedJobBase
    {
        public static int Delay { get; set; }

        private static readonly string jobPath;

        private static readonly string basePath;

        private static readonly string snapshotPath;

        private static readonly string delayPath;

        static FeedJob()
        {
            Delay = 1;

            basePath = AppDomain.CurrentDomain.BaseDirectory;

            jobPath = Path.Combine(basePath, "jobs");
            if (!Directory.Exists(jobPath))
                Directory.CreateDirectory(jobPath);

            snapshotPath = Path.Combine(basePath, "snapshot");

            if (!Directory.Exists(snapshotPath))
                Directory.CreateDirectory(snapshotPath);

            delayPath = Path.Combine(basePath, "delay");

            if (!Directory.Exists(delayPath))
                Directory.CreateDirectory(delayPath);
        }

        /// <summary>
        /// get feed request
        /// </summary>
        /// <returns>feed request list</returns>
        protected override List<FeedRequest> GetRequests()
        {
            Logger.GetLogger("").Info("start get feed");

            try
            {
                var requests = new List<FeedRequest>();
                var compile = new UrlCompile();
                var files = Directory.GetFiles(jobPath);

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file).ToLower();
                    if (extension != ".feed")
                        continue;

                    var parser = new RuiJiParser();
                    var result = parser.ParseFile(file);

                    if (result)
                    {
                        var request = parser.GetResult<Request>().Result;
                        var setting = parser.GetResult<FeedSetting>().Result;

                        if (request == null || setting == null)
                            continue;

                        var addrs = compile.GetResult(request.Uri.ToString());

                        for (int i = 0; i < addrs.Length; i++)
                        {
                            var addr = addrs[i].ToString();

                            var r = request.Clone() as Request;
                            r.Uri = new Uri(addr);
                            setting.Id += "_" + i;
                            r.Tag = JsonConvert.SerializeObject(setting);

                            var fr = new FeedRequest();
                            fr.Request = r;
                            fr.Setting = setting;
                            fr.Expression = parser.GetResult<ExtractBlock>().Expression;

                            requests.Add(fr);
                        }
                    }
                }

                return requests;
            }
            catch (Exception ex)
            {
                Logger.GetLogger("").Info("get feed error " + ex.Message);

                return new List<FeedRequest>();
            }
        }

        /// <summary>
        /// execute crawl
        /// </summary>
        /// <param name="feedRequest"></param>
        /// <returns>crawl response</returns>
        public override Response DoTask(FeedRequest feedRequest)
        {
            try
            {
                Logger.GetLogger("").Info("do task -> request address " + feedRequest.Request.Uri);

                var crawler = new RuiJiCrawler();
                var response = crawler.Request(feedRequest.Request);

                if(response != null)
                    Logger.GetLogger("").Info("request " + feedRequest.Request.Uri + " response code is " + response.StatusCode);
                if(response == null)
                    Logger.GetLogger("").Error("request " + feedRequest.Request.Uri + " response is null");

                return response;
            }
            catch (Exception ex)
            {
                Logger.GetLogger("").Info("do task -> request address failed " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// save feed snapshot
        /// </summary>
        /// <param name="feedRequest">feedRequest</param>
        /// <param name="response">crawl response</param>
        protected override void Save(FeedRequest feedRequest, Response response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return;

            if(response.IsRaw)
            {
                return;
            }

            var content = base.ConvertEncoding(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);
            var setting = JsonConvert.DeserializeObject<FeedSetting>(response.Request.Tag);

            var snapshot = new Snapshot
            {
                FeedId = setting.Id,
                RequestUrl = feedRequest.Request.Uri.ToString(),
                ResponseUrl = response.ResponseUri.ToString(),
                Content = content,
                Expression = feedRequest.Expression
            };

            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);

            var fileName = Path.Combine(snapshotPath, setting.Id + "_" + DateTime.Now.Ticks + ".json");

            if (setting.Delay > 0)
            {
                fileName = Path.Combine(delayPath, setting.Id + "_" + DateTime.Now.AddMinutes(setting.Delay).Ticks + ".json");
            }

            Logger.GetLogger("").Info(snapshot.RequestUrl + " response save to " + fileName);

            File.WriteAllText(fileName, json, Encoding.UTF8);
        }
    }
}