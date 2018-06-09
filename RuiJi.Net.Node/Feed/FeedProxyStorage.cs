using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Node.Db;
using RuiJi.Net.NodeVisitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed
{


    public class FeedProxyStorage : IStorage<ContentModel>
    {
        public bool Save(ContentModel t)
        {
            return Feeder.SaveContent(t);
        }
    }
}