using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter
{
    public class ExtractResult
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public ExtractResultCollection Tiles { get; set; }

        public ExtractResultCollection Blocks { get; set; }

        public Dictionary<string, ExtractResult> Metas { get; set; }

        public ExtractResult()
        {
            Tiles = new ExtractResultCollection();
            Blocks = new ExtractResultCollection();
            Metas = new  Dictionary<string, ExtractResult>();
        }
    }
}