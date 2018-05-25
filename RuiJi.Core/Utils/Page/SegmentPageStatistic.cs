using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Core.Utils.Page
{
    internal class SegmentPageStatistic
    {
        public int Start { get; set; }

        public int Count { get; set; }

        public int End
        {
            get
            {
                return Start + Count - 1;
            }
        }

        public SegmentPageStatistic()
        { }

        public SegmentPageStatistic(int start, int count)
        {
            this.Start = start;
            this.Count = count;
        }
    }
}
