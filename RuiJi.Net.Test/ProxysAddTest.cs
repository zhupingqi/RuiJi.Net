using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Node.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class ProxysAddTest
    {
        [TestMethod]
        public void Add()
        {
            var files = System.IO.Directory.GetFiles(@"d:\download");
            foreach (var item in files)
            {
                var f = System.IO.File.ReadAllText(item);

                var arr = JArray.Parse(f);
                foreach (var a in arr)
                {
                    var proxy = new ProxyModel();
                    proxy.Ip = a["metas"]["ip"].ToString();
                    proxy.Port = a["metas"]["port"].Value<int>();

                    ProxyLiteDb.AddOrUpdate(proxy);
                }
            }

        }
    }
}
