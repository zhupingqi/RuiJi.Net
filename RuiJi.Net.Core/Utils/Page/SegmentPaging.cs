using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Utils.Page
{
    public class SegmentPaging : _Paging
    {
        public string JsName { get; set; }
        #region 构造函数
        /// <summary>
        /// 构造默认分页大小
        /// </summary>
        public SegmentPaging()
            : this(10, 1,"doPage")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public SegmentPaging(int pageSize, int currentPage, string jsName = "doPage")
        {
            _currentPage = currentPage;
            PageSize = pageSize;
            JsName = jsName;
        }

        #endregion

        public List<SegmentPageResult> DoPaging(List<SegmentPage> segmentPage)
        {
            Count = segmentPage.Sum(m => m.Count);

            if (Start < 0)
                CurrentPage = 1;
            else if (Start > Count)
                CurrentPage = Pages;


            segmentPage = (from p in segmentPage
                           group p by p.Shard into g
                           select new SegmentPage() { Shard = g.Key, Count = g.Sum(m => m.Count), Value = g.FirstOrDefault().Value }).ToList();

            var ary = new List<SegmentPageStatistic>();

            var sum = 0;

            foreach (var seg in segmentPage)
            {
                var start = sum;
                var s = new SegmentPageStatistic(start, seg.Count);
                ary.Add(s);
                sum = s.End + 1;
            }

            var result = new List<SegmentPageResult>();

            var first = ary.FirstOrDefault(m => m.End >= Start);
            var firstIndex = ary.IndexOf(first);
            if (firstIndex < 0)
            {
                return result;
            }
            var last = ary.FirstOrDefault(m => m.End >= (Start + PageSize - 1));
            var lastIndex = ary.IndexOf(last);
            if (lastIndex < 0)
            {
                lastIndex = ary.Count - 1;
            }
            var count = lastIndex - firstIndex;

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                var sr = new SegmentPageResult();
                if (i == firstIndex)
                {
                    var row = first.End - Start + 1;
                    sr.Start = Start - first.Start;
                    sr.Rows = row > PageSize ? PageSize : row;
                    sr.Shard = segmentPage[i].Shard;
                    sr.Value = segmentPage[i].Value;
                    result.Add(sr);
                }
                else if (i == lastIndex)
                {
                    var row = (PageSize * CurrentPage) - ary[i].Start;
                    sr.Start = 0;
                    sr.Rows = row > ary[i].Count ? ary[i].Count : row;
                    sr.Shard = segmentPage[i].Shard;
                    sr.Value = segmentPage[i].Value;
                    result.Add(sr);
                }
                else
                {
                    sr.Start = 0;
                    sr.Rows = ary[i].Count;
                    sr.Shard = segmentPage[i].Shard;
                    sr.Value = segmentPage[i].Value;
                    result.Add(sr);
                }
            }

            return result;
        }
    }
}