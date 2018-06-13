using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage
{
    public class WebApiStorage : IStorage<IContentModel>
    {
        public string ConnectString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CreateIndex(string field, bool unique = false)
        {
            throw new NotImplementedException();
        }

        public int Insert(IContentModel content)
        {
            throw new NotImplementedException();
        }

        public int Insert(IContentModel[] contents)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(IContentModel content)
        {
            throw new NotImplementedException();
        }
    }
}
