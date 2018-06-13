using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public interface IStorage<T> where T : IContentModel
    {
        string ConnectString { get; set; }

        int Insert(T content);

        int Insert(T[] contents);

        bool Update(T content);

        bool Remove(int id);

        void CreateIndex(string field, bool unique = false);
    }
}