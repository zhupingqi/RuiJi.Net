using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Utils.Page
{
    public class SegmentPage
    {
        public int Count { get; set; }

        public string Shard { get; set; }

        public object Value { get; set; }

        public SegmentPage()
        { 
            
        }

        public SegmentPage(int count,string shard,object value = null)
        {
            if (count <= 0)
                throw new ArgumentException("Count must > 0");

            this.Count = count;
            this.Value = value;
            this.Shard = shard;
        }
    }
}