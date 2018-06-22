using MongoDB.Driver;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public class MongoDBStorage : StorageBase<IStorageModel>
    {
        public MongoDBStorage(string connectString, string databaseName, string collectionName) : base(connectString, databaseName, collectionName)
        {

        }

        public override int Insert(IStorageModel content)
        {
            var client = new MongoClient(ConnectString);
            var db = client.GetDatabase(DatabaseName);
            var collectionName = CollectionName;
            if (string.IsNullOrEmpty(collectionName))
                collectionName = content.GetType().FullName;

            var col = db.GetCollection<IStorageModel>(collectionName);

            col.InsertOne(content);
            
            return content.Id;
        }

        public override int Insert(IStorageModel[] contents)
        {
            if (contents.Length == 0)
                return 0;

            var client = new MongoClient(ConnectString);
            var db = client.GetDatabase(DatabaseName);
            var col = db.GetCollection<IStorageModel>(CollectionName);

            col.InsertMany(contents);

            return 0;
        }

        public override bool Remove(int id)
        {
            var client = new MongoClient(ConnectString);
            var db = client.GetDatabase(DatabaseName);
            var col = db.GetCollection<IStorageModel>(CollectionName);

            var result = col.DeleteOne(m=>m.Id == id);

            return result.DeletedCount > 0;
        }

        public override bool Remove(string url)
        {
            var client = new MongoClient(ConnectString);
            var db = client.GetDatabase(DatabaseName);
            var col = db.GetCollection<IStorageModel>(CollectionName);

            var result = col.DeleteOne(m => m.Url == url);

            return result.DeletedCount > 0;
        }

        public override bool Update(IStorageModel t)
        {
            Remove(t.Id);
            Remove(t.Url);
            Insert(t);

            return true;
        }
    }
}