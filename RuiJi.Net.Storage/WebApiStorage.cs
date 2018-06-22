using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public class WebApiStorage : IStorage<IStorageModel>
    {
        public string ConnectString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CreateIndex(string field, bool unique = false)
        {
            throw new NotImplementedException();
        }

        public int Insert(IStorageModel content)
        {
            throw new NotImplementedException();
        }

        public int Insert(IStorageModel[] contents)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(IStorageModel content)
        {
            throw new NotImplementedException();
        }
    }
}
