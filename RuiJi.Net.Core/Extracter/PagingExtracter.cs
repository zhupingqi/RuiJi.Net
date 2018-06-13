using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter
{
    public class PagingExtracter
    {
        public static ExtractResult Extract(Uri uri, ExtractResult result, ExtractBlock block)
        {
            var pages = new Dictionary<string, ExtractResult>();
            pages.Add(uri.ToString(), result);

            var lines = String.Join("\n", result.Paging);
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

                var r = RuiJiExtracter.Extract(content, block);
                pages.Add(u.ToString(), r);

                if (r.Paging != null && r.Paging.Count > 0)
                {
                    var nlines = String.Join("\n", r.Paging);
                    var diff = diffBuilder.BuildDiffModel(lines, nlines);

                    nlines = string.Join("\n", diff.Lines.Select(m => m.Text));
                    reader = new StringReader(nlines);
                    url = reader.ReadLine();
                }
            }

            return result;
        }
    }
}