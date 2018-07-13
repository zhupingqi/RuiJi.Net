using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.RTS;
using RuiJi.Net.Core.Utils.Log;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Compile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedJob : FeedJobBase
    {
        public static bool IsRunning = false;

        private static string baseDir;

        private static int _page = 0;

        private string baseUrl;

        private string proxyUrl;

        private FeedNode feedNode;

        static FeedJob()
        {
            baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(baseDir + @"snapshot"))
            {
                Directory.CreateDirectory(baseDir + @"snapshot");
            }

            if (!Directory.Exists(baseDir + @"delay"))
            {
                Directory.CreateDirectory(baseDir + @"delay");
            }
        }

        //public async Task Execute(IJobExecutionContext context)
        //{
        //    if (!IsRunning)
        //    {
        //        IsRunning = true;
        //        baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();
        //        proxyUrl = context.JobDetail.JobDataMap.Get("proxyUrl").ToString();
        //        feedNode = context.JobDetail.JobDataMap.Get("node") as FeedNode;

        //        Logger.GetLogger(baseUrl).Info("start feed job execute");

        //        var task = Task.Factory.StartNew(() =>
        //        {                   
        //            var feeds = GetFeedJobs(baseUrl,proxyUrl, feedNode);
        //            var compile = new UrlCompile();

        //            var stpStartInfo = new STPStartInfo
        //            {
        //                IdleTimeout = 3000,
        //                MaxWorkerThreads = 8,
        //                MinWorkerThreads = 0
        //            };

        //            var pool = new SmartThreadPool(stpStartInfo);
        //            var waits = new List<IWorkItemResult>();

        //            foreach (var feed in feeds)
        //            {
        //                var addrs = compile.GetResult(feed.Address);

        //                Logger.GetLogger(baseUrl).Info("compile address " + feed.Address + " result " + string.Join(",", addrs));

        //                foreach (var addr in addrs)
        //                {
        //                    feed.Address = addr.ToString();

        //                    var item = pool.QueueWorkItem((u) =>
        //                    {
        //                        DoTask(u, true);
        //                    }, feed);

        //                    waits.Add(item);
        //                }
        //            }

        //            SmartThreadPool.WaitAll(waits.ToArray());

        //            pool.Shutdown(true, 1000);
        //            pool.Dispose();
        //            pool = null;
        //            waits.Clear();
        //        });

        //        await task;

        //        IsRunning = false;

        //        Logger.GetLogger(baseUrl).Info("end feed job execute");
        //    }
        //}

        //private List<FeedModel> GetFeedJobs(string baseUrl,string proxyUrl, FeedNode node)
        //{
        //    Logger.GetLogger(baseUrl).Info("start get feed");

        //    try
        //    {
        //        if (NodeConfigurationSection.Standalone)
        //        {
        //            var paging = new Paging();
        //            paging.CurrentPage = _page;
        //            paging.PageSize = 50;

        //            var feeds = FeedLiteDb.GetAvailableFeeds(paging);
        //            if(_page > 0 && feeds.Count == 0)
        //            {
        //                _page = 0;
        //                paging.CurrentPage = _page;
        //                feeds = FeedLiteDb.GetAvailableFeeds(paging);
        //            }

        //            _page++;

        //            Logger.GetLogger(baseUrl).Info("get feed jobs:" + feeds.Count);

        //            return feeds;
        //        }
        //        else
        //        {
        //            var d = node.GetData("/config/feed/" + node.BaseUrl);
        //            var config = JsonConvert.DeserializeObject<NodeConfig>(d.Data);
        //            var pages = config.Pages == null ? "" : string.Join(",", config.Pages);

        //            var client = new RestClient("http://" + proxyUrl);
        //            var restRequest = new RestRequest("api/feed/job?pages=" + pages);
        //            restRequest.Method = Method.GET;

        //            var restResponse = client.Execute(restRequest);

        //            var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(restResponse.Content);

        //            Logger.GetLogger(baseUrl).Info("get feed jobs:" + feeds.Count);

        //            return feeds;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetLogger(baseUrl).Info("get feed error " + ex.Message);

        //        return new List<FeedModel>();
        //    }
        //}

        //public FeedSnapshot DoTask(FeedModel feed,bool persistence = false)
        //{
        //    try
        //    {
        //        Logger.GetLogger(baseUrl).Info("do task -> request address " + feed.Address);

        //        var request = new Request(feed.Address);
        //        request.RunJS = (feed.RunJS == Status.ON);
        //        if (feed.Headers != null)
        //        {
        //            request.Headers = feed.Headers;

        //            if (request.Headers.Count(m => m.Name == "Referer") == 0)
        //                request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
        //        }

        //        request.Method = feed.Method;
        //        if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.Data))
        //            request.Data = feed.Data;

        //        var ua = UALiteDb.GetOne();
        //        if(!string.IsNullOrEmpty(ua))
        //            request.Headers.Add(new WebHeader("User-Agent", ua));

        //        var response = NodeVisitor.Crawler.Request(request);

        //        if(response != null)
        //            Logger.GetLogger(baseUrl).Info("request " + feed.Address + " response code is " + response.StatusCode);
        //        if(response == null)
        //            Logger.GetLogger(baseUrl).Error("request " + feed.Address + " response is null");

        //        if (response != null && response.StatusCode == HttpStatusCode.OK)
        //        {
        //            var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

        //            var snap = new FeedSnapshot
        //            {
        //                Url = feed.Address,
        //                Content = content,
        //                Type = feed.Type,
        //                BlockExpression = feed.BlockExpression,
        //                RuiJiExpression = feed.RuiJiExpression
        //            };

        //            if (persistence)
        //            {
        //                var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

        //                var fileName = baseDir + @"snapshot\" + feed.Id + "_" + DateTime.Now.Ticks + ".json";
        //                if (feed.Delay > 0)
        //                {
        //                    fileName = baseDir + @"delay\" + feed.Id + "_" + DateTime.Now.AddMinutes(feed.Delay).Ticks + ".json";
        //                }

        //                Logger.GetLogger(baseUrl).Info(feed.Address + " response save to " + fileName);
        //                File.WriteAllText(fileName, json, Encoding.UTF8);
        //            }

        //            return snap;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetLogger(baseUrl).Info("do task -> request address failed " + ex.Message);
        //    }

        //    return null;
        //}

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }

        protected override void OnJobStart(IJobExecutionContext context)
        {
            baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();
            proxyUrl = context.JobDetail.JobDataMap.Get("proxyUrl").ToString();
            feedNode = context.JobDetail.JobDataMap.Get("node") as FeedNode;
        }

        protected override List<FeedRequest> GetRequests()
        {
            Logger.GetLogger(baseUrl).Info("start get feed");

            try
            {
                if (NodeConfigurationSection.Standalone)
                {
                    var paging = new Paging();
                    paging.CurrentPage = _page;
                    paging.PageSize = 50;

                    var feeds = FeedLiteDb.GetAvailableFeeds(paging);
                    if (_page > 0 && feeds.Count == 0)
                    {
                        _page = 0;
                        paging.CurrentPage = _page;
                        feeds = FeedLiteDb.GetAvailableFeeds(paging);
                    }

                    _page++;

                    Logger.GetLogger(baseUrl).Info("get feed jobs:" + feeds.Count);

                    return GetFeedRequest(feeds);
                }
                else
                {
                    var d = feedNode.GetData("/config/feed/" + feedNode.BaseUrl);
                    var config = JsonConvert.DeserializeObject<NodeConfig>(d.Data);
                    var pages = config.Pages == null ? "" : string.Join(",", config.Pages);

                    var client = new RestClient("http://" + proxyUrl);
                    var restRequest = new RestRequest("api/feed/job?pages=" + pages);
                    restRequest.Method = Method.GET;

                    var restResponse = client.Execute(restRequest);

                    var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(restResponse.Content);

                    Logger.GetLogger(baseUrl).Info("get feed jobs:" + feeds.Count);

                    return GetFeedRequest(feeds);
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger(baseUrl).Info("get feed error " + ex.Message);

                return new List<FeedRequest>();
            }
        }

        public Response DoTask(FeedModel feed)
        {
            return DoTask(FeedModelToFeedRequest(feed));
        }

        public override Response DoTask(FeedRequest feedRequest)
        {
            try
            {
                var request = feedRequest.Request;

                Logger.GetLogger(baseUrl).Info("do task -> request address " + request.Uri);

                var response = NodeVisitor.Crawler.Request(request);

                if (response != null)
                    Logger.GetLogger(baseUrl).Info("request " + request.Uri + " response code is " + response.StatusCode);

                if (response == null)
                    Logger.GetLogger(baseUrl).Error("request " + request.Uri + " response is null");

                return response;
            }
            catch (Exception ex)
            {
                Logger.GetLogger(baseUrl).Info("do task -> request address failed " + ex.Message);
            }

            return null;
        }

        protected override void Save(FeedRequest feedRequest, Response response)
        {
            var request = feedRequest.Request;
            var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

            var snap = new FeedSnapshot
            {
                Url = request.Uri.ToString(),
                Content = content,
                RuiJiExpression = feedRequest.Expression
            };

            var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

            var fileName = baseDir + @"snapshot\" + feedRequest.Setting.Id + "_" + DateTime.Now.Ticks + ".json";
            if (feedRequest.Setting.Delay > 0)
            {
                fileName = baseDir + @"delay\" + feedRequest.Setting.Id + "_" + DateTime.Now.AddMinutes(feedRequest.Setting.Delay).Ticks + ".json";
            }

            Logger.GetLogger(baseUrl).Info(request.Uri + " response save to " + fileName);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        protected List<FeedRequest> GetFeedRequest(List<FeedModel> feeds)
        {
            var requests = new List<FeedRequest>();

            foreach (var feed in feeds)
            {
                requests.Add(FeedModelToFeedRequest(feed));
            }

            return requests;
        }

        private FeedRequest FeedModelToFeedRequest(FeedModel feed)
        {
            var request = new Request(feed.Address);
            request.RunJS = (feed.RunJS == Status.ON);
            if (feed.Headers != null)
            {
                request.Headers = feed.Headers;

                if (request.Headers.Count(m => m.Name == "Referer") == 0)
                    request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
            }

            request.Method = feed.Method;
            if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.Data))
            {
                request.ContentType = feed.ContentType;
                request.Data = feed.Data;
            }
            var ua = UALiteDb.GetOne();
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
    }
}