using Newtonsoft.Json.Linq;
using RuiJi.Net.Node.Feed.Db;
using Xunit;

namespace RuiJi.Net.Test
{

    public class ProxysAddTest
    {
        [Fact]
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
