using DiffPlex;
using DiffPlex.DiffBuilder;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Visitor
    {
        public Response Request(string url, string method = "GET",string ip = "")
        {
            var request = new Request(url);
            request.Method = method;

            var cralwer = new Crawler();
            if (!string.IsNullOrEmpty(ip))
                request.Ip = ip;

            return cralwer.Request(url);
        }

        public Response Request(Request request)
        {
            var cralwer = new Crawler();
            return cralwer.Request(request);
        }

        public ExtractResult Extract(string url, string method = "GET", string ip = "")
        {
            var request = new Request(url);
            request.Method = method;

            var cralwer = new Crawler();
            if (!string.IsNullOrEmpty(ip))
                request.Ip = ip;

            var response = cralwer.Request(request);
            var content = response.Data.ToString();

            var blocks = Feeder.GetExtractBlock(url);
            var er = new ExtractRequest
            {
                Blocks = blocks,
                Content = content
            };

            var results = Extracter.Extract(er);

            var result = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();

            if (result.Paging != null && result.Paging.Count > 0 && result.Metas != null && result.Metas.ContainsKey("content"))
            {
                result = Extract(new Uri(url), result, request.Method, request.Ip);
            }

            return result;
        }

        public static ExtractResult Extract(Uri uri, ExtractResult result, string method, string ip)
        {
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

                var cralwer = new Crawler();
                if (!string.IsNullOrEmpty(ip))
                    request.Ip = ip;

                var response = cralwer.Request(request);
                var content = response.Data.ToString();

                var blocks = Feeder.GetExtractBlock(u.ToString());
                var er = new ExtractRequest
                {
                    Blocks = blocks,
                    Content = content
                };

                var results = Extracter.Extract(er);

                var r = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();
                result.Metas["content"] = result.Metas["content"].ToString() + r.Metas["content"].ToString();

                if (r.Paging != null && r.Paging.Count > 0)
                {
                    var nlines = String.Join("\n", r.Paging.Distinct());
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