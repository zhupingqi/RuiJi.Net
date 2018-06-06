using RuiJi.Net.Core.Extracter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core
{
    public interface IContentModel
    {
        int FeedId { get; set; }

        string Url { get; set; }
    }

    public interface IStorage<T> where T : IContentModel
    {
        bool Save(T t);
    }
}