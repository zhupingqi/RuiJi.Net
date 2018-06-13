using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public abstract class StorageBase<T> : IStorage<T> where T : IContentModel
    {
        public string ConnectString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public StorageBase(string connectString,string databaseName, string collectionName)
        {
            this.ConnectString = connectString;
            this.DatabaseName = databaseName;
            this.CollectionName = collectionName;
        }

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