using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Node;
using RuiJi.Net.NodeVisitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RuiJi.Net.Owin
{
    public class NodeRouteAttribute : ActionFilterAttribute
    {
        public NodeTypeEnum Target { get; set; }

        public string RouteArgumentName { get; set; }

        public NodeRouteAttribute()
        {

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var node = ServerManager.Get(actionContext.Request.RequestUri.Authority);
            if (node == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(actionContext.Request.RequestUri.Authority + " no node mapping this uri");
            }

            if ((int)node.NodeType != (int)Target)
            {
                var baseUrl = "";

                if ((int)Target < 3)
                {
                     baseUrl = ProxyManager.Instance.Elect((NodeProxyTypeEnum)Target);                    
                }
                else
                {
                    baseUrl = actionContext.ActionArguments[RouteArgumentName].ToString();
                }

                var client = new RestClient("http://" + baseUrl);
                var restRequest = new RestRequest(actionContext.Request.RequestUri.PathAndQuery);
                restRequest.Method = (actionContext.Request.Method == HttpMethod.Get) ? Method.GET : Method.POST;
                restRequest.JsonSerializer = new NewtonJsonSerializer();
                if (restRequest.Method == Method.POST)
                {
                    foreach (var arg in actionContext.ActionArguments)
                    {
                        restRequest.AddJsonBody(arg.Value);
                    }
                }

                var restResponse = client.Execute(restRequest);

                if (actionContext.ActionDescriptor.ReturnType != null)
                {
                    var obj = JsonConvert.DeserializeObject<object>(restResponse.Content);

                    actionContext.Response = actionContext.Request.CreateResponse(obj);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse();
                }
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }
    }
}