using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public abstract class StorageBase<T> : IStorage<T> where T : IStorageModel
    {       
        public abstract int Insert(T content);

        public abstract int Insert(T[] contents);

        public abstract bool Remove(int id);

        public abstract bool Remove(string url);

        public abstract bool Update(T content);

        public virtual void CreateIndex(string field, bool unique = false)
        {
        }
    }
}