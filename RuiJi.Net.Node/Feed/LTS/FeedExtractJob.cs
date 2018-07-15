using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.RTS;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedExtractJob : FeedExtractJobBase<string>
    {
        private static readonly string snapshotPath;

        private static readonly string basePath;

        private static readonly string failPath;

        private static string baseUrl;

        private static SmartThreadPool smartThreadPool;

        static FeedExtractJob()
        {
            snapshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapshot");

            basePath = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(basePath + @"/history"))
            {
                Directory.CreateDirectory(basePath + @"/history");
            }

            if (!Directory.Exists(basePath + @"/pre"))
            {
                Directory.CreateDirectory(basePath + @"/pre");
            }

            failPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "save_failed");
            if (!Directory.Exists(failPath))
                Directory.CreateDirectory(failPath);

            var stpStartInfo = new STPStartInfo
            {
                IdleTimeout = 3000,
                MaxWorkerThreads = 32,
                MinWorkerThreads = 0
            };

            smartThreadPool = new SmartThreadPool(stpStartInfo);
        }

        protected override void OnJobStart(IJobExecutionContext context)
        {
            baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();

            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"delay");
            foreach (var file in files)
            {
                var filename = file.Substring(file.LastIndexOf(@"\") + 1);
                var sp = filename.Split('_');
                var ticks = sp[1].Substring(0, sp[1].LastIndexOf("."));

                if (long.Parse(ticks) < DateTime.Now.Ticks)
                {
                    var desFile = file.Replace("delay", "snapshot");
                    File.Move(file, desFile);

                    Logger.GetLogger(baseUrl).Info("move delay feed " + file);
                }
            }
        }

        public List<string> ExtractAddress(FeedSnapshot feed)
        {
            var block = new ExtractBlock();
            block.TileSelector.Selectors.Add(new CssSelector("a", "href"));

            if (feed.UseBlock)
            {
                if (!string.IsNullOrEmpty(feed.BlockExpression))
                    block = JsonConvert.DeserializeObject<ExtractBlock>(feed.BlockExpression);
            }
            else
            {
                if (!string.IsNullOrEmpty(feed.RuiJiExpression))
                {
                    block.TileSelector.Selectors.Clear();

                    var parser = new RuiJiParser();

                    var s = RuiJiBlockParser.ParserBase(feed.RuiJiExpression).Selectors;
                    block.TileSelector.Selectors.AddRange(s);
                }
            }

            var result = RuiJiExtractor.Extract(feed.Content, block);
            var results = new List<string>();

            if (result.Tiles != null)
            {
                foreach (var item in result.Tiles)
                {
                    var href = item.Content.ToString();
                    if (href.Contains("#"))
                    {
                        href = href.Substring(0, href.IndexOf('#'));
                    }
                    if (Uri.IsWellFormedUriString(href, UriKind.Relative))
                        href = new Uri(new Uri(feed.Url), href).AbsoluteUri.ToString();
                    results.Add(href);
                }
            }

            return results.Distinct().ToList();
        }

        protected override List<string> GetSnapshot()
        {
            return Directory.GetFiles(snapshotPath).ToList();
        }

        public override void DoTask(string path)
        {
            try
            {
                var filename = path.Substring(path.LastIndexOf(@"\") + 1);
                var sp = filename.Split('_');
                var feedId = Convert.ToInt32(sp[0]);
                var content = File.ReadAllText(path);

                var feed = JsonConvert.DeserializeObject<FeedSnapshot>(content);
                var url = feed.Url;

                var urls = ExtractAddress(feed);

                var hisFile = AppDomain.CurrentDomain.BaseDirectory + @"history\" + feedId + ".txt";
                var urlsHistory = new string[0];
                if (File.Exists(hisFile))
                {
                    urlsHistory = File.ReadAllLines(hisFile, Encoding.UTF8);
                }

                File.WriteAllLines(hisFile, urls, Encoding.UTF8);

                urls.RemoveAll(m => urlsHistory.Contains(m));
                urls.RemoveAll(m => string.IsNullOrEmpty(m));
                urls.RemoveAll(m => !Uri.IsWellFormedUriString(m, UriKind.Absolute));

                foreach (var u in urls)
                {
                    Logger.GetLogger(baseUrl).Info(" extract job " + u + " add to feed extract queue");

                    var r = smartThreadPool.QueueWorkItem(() => {
                        Logger.GetLogger(baseUrl).Info(" extract job " + u + " starting");

                        var result = NodeVisitor.Cooperater.GetResult(u);
                        if (result != null)
                        {
                            Save(feedId, u, result);
                        }
                    });

                    SmartThreadPool.WaitAll(new List<IWorkItemResult> { r }.ToArray());
                }

                var destFile = path.Replace("snapshot", "pre").Replace(filename, feedId + ".txt");
                if (File.Exists(destFile))
                    File.Delete(destFile);

                File.Move(path, destFile);
            }
            catch { }
        }

        protected void Save(int feedId,string url, ExtractResult result)
        {
            var cm = new ContentModel();
            cm.FeedId = feedId;
            cm.Url = url;
            cm.Metas = result.Metas;
            cm.CDate = DateTime.Now;

            var connectString = string.Format(@"LiteDb/Content/{0}.db", DateTime.Now.ToString("yyyyMM"));
            var storage = new LiteDbStorage(connectString, "contents");

            if (storage.Insert(cm) == -1)
                File.AppendAllText(failPath + @"\" + EncryptHelper.GetMD5Hash(url) + ".json", JsonConvert.SerializeObject(cm));

            Logger.GetLogger(baseUrl).Info(" extract job " + url + " save result " + (storage.Insert(cm) != -1));
        }
    }
}