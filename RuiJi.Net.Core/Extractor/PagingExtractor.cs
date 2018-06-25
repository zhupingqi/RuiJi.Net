using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    public class PagingExtractor
    {
        public delegate void PageDownloadHandler(Uri uri, ExtractResult result);

        public static ExtractResult MergeContent(Uri uri, ExtractResult result, ExtractBlock block, int maxRetry = 10)
        {
            var content = "";

            DownloadPage(uri, result, block, (u,r) => {
                content += r.Metas["content"].ToString();
            });

            result.Metas["content"] = content;

            return result;
        }

        public static void DownloadPage(Uri uri, ExtractResult result, ExtractBlock block, PageDownloadHandler handler, int maxRetry = 10)
        {
            handler(uri,result);

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

                var response = crawler.Request(request);
                var content = response.Data.ToString();

                var r = RuiJiExtractor.Extract(content, block);
                if (r.Paging == null || r.Paging.Count == 0)
                {
                    Thread.Sleep(5000);
                    if (--maxRetry == 0)
                        break;

                    continue;
                }

                pages.Add(u.ToString(), r);
                handler(u, r);

                var nlines = String.Join("\n", r.Paging.Distinct());
                var diff = diffBuilder.BuildDiffModel(lines, nlines);

                nlines = string.Join("\n", diff.Lines.Select(m => m.Text));
                reader = new StringReader(nlines);
                url = reader.ReadLine();
            }
        }
    }
}