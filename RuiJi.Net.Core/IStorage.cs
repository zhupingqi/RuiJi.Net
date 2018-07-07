using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    /// <summary>
    /// storage model interface
    /// </summary>
    public interface IStorageModel
    {
        int Id { get; set; }

        DateTime CDate { get; set; }

        string Url { get; set; }
    }

    /// <summary>
    /// storage interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStorage<T> where T : IStorageModel
    {
        string ConnectString { get; set; }

        int Insert(T content);

        int Insert(T[] contents);

        bool Update(T content);

        bool Remove(int id);

        void CreateIndex(string field, bool unique = false);
    }
}