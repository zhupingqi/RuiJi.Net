using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;

namespace RuiJi.Net.Owin.Models
{
    public class CrawlTaskFunc : IParallelTaskFunc
    {
        public object Run(object t, ParallelTask task)
        {
            var model = t as CrawlTaskModel;

            var results = new List<object>();
            var reporter = task.Progress as IProgress<string>;

            reporter.Report("正在读取Feed记录");
            var feed = FeedLiteDb.GetFeed(model.FeedId);

            reporter.Report("正在下载 Feed");

            //var compile = new Node.Compile.JSUrlCompile();
            var addrs = CodeCompilerManager.GetResult("url", feed.Address); //compile.GetResult(feed.Address);

            foreach (var addr in addrs)
            {
                feed.Address = addr.ToString();

                var job = new FeedJob();
                var response = job.DoTask(feed);
                reporter.Report("Feed 下载完成");

                var block = RuiJiBlockParser.ParserBlock(feed.RuiJiExpression);

                var feedResult = RuiJiExtractor.Extract(response.Data.ToString(), block);
                results.Add(feedResult);

                var snap = new FeedSnapshot
                {
                    Url = feed.Address,
                    Content = response.Data.ToString(),
                    Type = feed.Type,
                    RuiJiExpression = feed.RuiJiExpression
                };

                reporter.Report("正在提取Feed地址");
                var j = new FeedExtractJob();
                var urls = j.ExtractAddress(snap);
                reporter.Report("Feed地址提取完成");

                if (!string.IsNullOrEmpty(snap.RuiJiExpression))
                {
                    foreach (var url in urls)
                    {
                        reporter.Report("正在提取地址 " + url);
                        var result = Cooperater.GetResult(url);

                        if (result != null)
                        {
                            var cm = new ContentModel();
                            cm.Id = model.FeedId;
                            cm.Url = url;
                            cm.Metas = result.Metas;
                            cm.CDate = DateTime.Now;

                            results.Add(cm);
                        }
                    }
                }

                reporter.Report("计算完成");

                if (!model.IncludeContent)
                {
                    results.ForEach((m) =>
                    {
                        ClearContent(m);
                    });
                }
            }

            return results;
        }

        public static void ClearContent(object obj)
        {
            var result = obj as ExtractResult;
            if (result == null)
                return;

            if (result.Blocks != null || result.Metas != null || result.Tiles != null)
            {
                result.Content = null;
            }

            if (result.Tiles != null)
            {
                foreach (var tile in result.Tiles)
                {
                    if (tile.Metas != null)
                        tile.Content = null;
                }
            }

            if (result.Blocks != null && result.Blocks.Count > 0)
            {
                result.Blocks.ForEach((m) =>
                {
                    ClearContent(m);
                });
            }
        }
    }
}