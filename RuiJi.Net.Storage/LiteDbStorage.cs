using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public class LiteDbStorage : StorageBase
    {
        public LiteDbStorage(string connectString, string collectionName) : base(connectString, "", collectionName)
        {

        }

        public override int Insert(IContentModel content)
        {
            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);

                var c = col.Find(m => m.Url == content.Url).FirstOrDefault();
                if (c == null)
                {
                    content.CDate = DateTime.Now;
                    var id = col.Insert(content).AsInt32;

                    return id;
                }
            }

            return -1;
        }

        public override int Insert(IContentModel[] contents)
        {
            var count = 0;

            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);

                foreach (var content in contents)
                {
                    var c = col.Find(m => m.Url == content.Url).FirstOrDefault();
                    if (c == null)
                    {
                        content.CDate = DateTime.Now;
                        col.Insert(content);

                        count++;
                    }
                }                    
            }

            return count > 0 ? count : -1;
        }

        public override bool Remove(int id)
        {
            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);

                return col.Delete(id);
            }
        }

        public override bool Remove(string url)
        {
            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);
                var f = col.Find(m => m.Url == url);
                if(f.Count() > 0)
                    return col.Delete(f.First().Id);
            }

            return false;
        }

        public override bool Update(IContentModel content)
        {
            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);

                if (content.Id > 0)
                {
                    return col.Update(content);
                }
            }

            return false;
        }

        public override void CreateIndex(string field, bool unique = false)
        {
            using (var db = new LiteDatabase(ConnectString))
            {
                var col = db.GetCollection<IContentModel>(CollectionName);
                col.EnsureIndex(m => m.Id);
                col.EnsureIndex(m => m.Url, true);
                col.EnsureIndex(field, unique);
            }
        }
    }
}