using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Core.Utils.Page
{
    public class SegmentPageResult
    {
        public int Start { get; set; }

        public int Rows { get; set; }

        public string Shard { get; set; }

        public object Value { get; set; }
    }
}