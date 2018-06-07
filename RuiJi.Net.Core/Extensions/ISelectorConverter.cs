using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extensions
{
    [JsonArray]
    public class ISelectorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType is ISelector;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject.SelectToken("type").ToString();
            var json = jsonObject.ToString();

            switch(type)
            {
                case "0":
                    {
                        return JsonConvert.DeserializeObject<CssSelector>(json);
                    }
                case "1":
                    {
                        return JsonConvert.DeserializeObject<RegexSelector>(json);
                    }
                case "2":
                    {
                        return JsonConvert.DeserializeObject<RegexSplitSelector>(json);
                    }
                case "3":
                    {
                        return JsonConvert.DeserializeObject<TextRangeSelector>(json);
                    }
                case "4":
                    {
                        return JsonConvert.DeserializeObject<ExcludeSelector>(json);
                    }
                case "5":
                    {
                        return null;
                    }
                case "6":
                    {
                        return JsonConvert.DeserializeObject<RegexReplaceSelector>(json);
                    }
                case "7":
                    {
                        return JsonConvert.DeserializeObject<JsonPathSelector>(json);
                    }
                case "8":
                    {
                        return JsonConvert.DeserializeObject<XPathSelector>(json);
                    }
                case "9":
                    {
                        return JsonConvert.DeserializeObject<ClearTagSelector>(json);
                    }
                case "10":
                    {
                        return JsonConvert.DeserializeObject<ExpressionSelector>(json);
                    }
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
