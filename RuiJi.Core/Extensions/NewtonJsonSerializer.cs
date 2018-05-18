using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extensions
{
    public class NewtonJsonSerializer : ISerializer, IDeserializer
    {
        public string ContentType
        {
            get;
            set;
        }

        public string DateFormat
        {
            get;
            set;
        }

        public string Namespace
        {
            get;
            set;
        }

        public string RootElement
        {
            get;
            set;
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public NewtonJsonSerializer()
        {
            ContentType = "application/json";
        }
    }
}
