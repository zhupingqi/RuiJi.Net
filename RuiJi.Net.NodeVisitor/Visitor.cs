using RuiJi.Net.Core.Extracter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Visitor
    {
        public List<ExtractResult> Extract(string url)
        {
            var cralwer = new Crawler();
            var response = cralwer.Request(url);
            var content = response.Data.ToString();

            var results = new List<ExtractResult>();

            var blocks = Feeder.GetExtractBlock(url);
            blocks.ForEach((m) => {
                var r = Extracter.Extract(new ExtractRequest
                {
                    Block = m,
                    Content = content
                });

                results.Add(r);
            });

            return results;
        }
    }
}