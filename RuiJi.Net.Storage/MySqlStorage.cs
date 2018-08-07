using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public class MySqlStorage : StorageBase<IStorageModel>
    {
        public MySqlStorage(string connectString, string databaseName = "", string collectionName = "")
        {
        }

        public override int Insert(IStorageModel content)
        {
            throw new NotImplementedException();
        }

        public override int Insert(IStorageModel[] contents)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(string url)
        {
            throw new NotImplementedException();
        }

        public override bool Update(IStorageModel content)
        {
            throw new NotImplementedException();
        }
    }
}
