using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RuiJi.Net.Core.Code.Provider;
using RuiJi.Net.Node.Feed.Db;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RuiJi.Net.Node.Compile
{
    public class RemoteCodeProvider : ICodeProvider
    {
        private Dictionary<string, FuncModel> functions;
        private bool init = false;
        private string baseUrl;

        private readonly NodeBase nodeBase;
        private readonly string funcType;

        public RemoteCodeProvider(NodeBase node, string type = "")
        {
            functions = new Dictionary<string, FuncModel>();
            this.nodeBase = node;
            this.funcType = type;
        }

        private bool Init()
        {
            var nodes = nodeBase.GetLiveNode();
            var node = nodes.First(m => m.Data == "feed proxy");
            if (node == null)
                return false;

            baseUrl = node.Path.Substring(node.Path.LastIndexOf("/"));

            var offset = 0;
            var limit = 100;

            var results = QueryPage(offset, limit);
            while (results.Count > 0)
            {
                foreach (var r in results)
                {
                    functions.Add(r.Name, r);
                }

                offset += limit;
                results = QueryPage(offset, limit);
            }

            return true;
        }

        private List<FuncModel> QueryPage(int offset, int limit)
        {
            var client = new RestClient("http://" + baseUrl);
            var restRequest = new RestRequest("api/setting/func/list?offset=" + offset + "&limit=" + limit + "&type=" + funcType);
            restRequest.Method = Method.GET;

            var resetEvent = new ManualResetEvent(false);
            var results = new List<FuncModel>();

            var handle = client.ExecuteAsync(restRequest, (restResponse) =>
            {
                var obj = JObject.Parse(restResponse.Content);

                results = JsonConvert.DeserializeObject<List<FuncModel>>(obj.GetValue("list").ToString());

                resetEvent.Set();
            });

            resetEvent.WaitOne();
            return results;
        }

        private FuncModel Query(string name)
        {
            var client = new RestClient("http://" + baseUrl);
            var restRequest = new RestRequest("api/setting/func/name=" + name);
            restRequest.Method = Method.GET;

            var resetEvent = new ManualResetEvent(false);
            FuncModel funcModel = null;

            var handle = client.ExecuteAsync(restRequest, (restResponse) =>
            {
                funcModel = JsonConvert.DeserializeObject<FuncModel>(restResponse.Content);

                resetEvent.Set();
            });

            resetEvent.WaitOne();

            if (funcModel != null)
            {
                functions.Add(funcModel.Name, funcModel);
            }

            return funcModel;
        }

        public string GetCode(string name)
        {
            if(!init && Init())
            {
                init = true;
            }

            if (!init)
                return "";

            if (functions.ContainsKey(name))
            {
                return functions[name].Code;
            }

            lock (this)
            {
                var f = Query(name);
                if (f != null)
                    return f.Code;
            }

            return "";
        }
    }
}