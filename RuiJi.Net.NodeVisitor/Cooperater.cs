using DiffPlex;
using DiffPlex.DiffBuilder;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Cooperater
    {
        public delegate void PageDownloadHandler(Uri uri, ExtractResult result);

        public static ExtractResult GetResult(string url, string method = "GET", string ip = "")
        {
            var request = new Request(url);
            request.Method = method;
            request.Ip = ip;

            return GetResult(request);
        }

        public static ExtractResult GetResult(Request request)
        {
            var response = Crawler.Request(request);
            var content = response.Data.ToString();

            var blocks = Feeder.GetExtractBlock(request.Uri.ToString());
            var er = new ExtractRequest
            {
                Blocks = blocks,
                Content = content
            };

            var results = Extracter.Extract(er);

            var result = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();

            if (result.Paging != null && result.Paging.Count > 0 && result.Metas != null && result.Metas.ContainsKey("content"))
            {
                result = MergeContent(request.Uri, result, request.Method, request.Ip);
            }

            return result;
        }

        public static ExtractResult MergeContent(Uri uri, ExtractResult result, string method, string ip, int maxRetry = 10)
        {
            var content = "";

            DownloadPage(uri, result, method, ip, (u, r) =>
            {
                content += r.Metas["content"].ToString();
            }, maxRetry);

            result.Metas["content"] = content;

            return result;
        }

        public static void DownloadPage(Uri uri, ExtractResult result, string method, string ip, PageDownloadHandler handler, int maxRetry = 10)
        {
            handler(uri, result);

            var pages = new Dictionary<string, ExtractResult>();
            pages.Add(uri.ToString(), result);

            var lines = String.Join("\n", result.Paging.Distinct());
            var reader = new StringReader(lines);

            var crawler = new RuiJiCrawler();

            var url = reader.ReadLine();

            var diffBuilder = new InlineDiffBuilder(new Differ());

            while (!string.IsNullOrEmpty(url))
            {
                var u = new Uri(uri, url);
                if (pages.ContainsKey(u.ToString()))
                {
                    url = reader.ReadLine();
                    continue;
                }

                var request = new Request(u);
                request.Method = method;

                if (!string.IsNullOrEmpty(ip))
                    request.Ip = ip;

                var response = Crawler.Request(request);
                var content = response.Data.ToString();

                var blocks = Feeder.GetExtractBlock(u.ToString());
                var er = new ExtractRequest
                {
                    Blocks = blocks,
                    Content = content
                };

                var results = Extracter.Extract(er);

                var r = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();
                if (r.Paging == null)
                {
                    Thread.Sleep(3000);
                    if (--maxRetry == 0)
                        break;

                    continue;
                }

                handler(uri, result);

                if (r.Paging != null && r.Paging.Count > 0)
                {
                    var nlines = String.Join("\n", r.Paging.Distinct());
                    var diff = diffBuilder.BuildDiffModel(lines, nlines);

                    nlines = string.Join("\n", diff.Lines.Select(m => m.Text));
                    reader = new StringReader(nlines);
                    url = reader.ReadLine();
                }
            }
        }
    }
}