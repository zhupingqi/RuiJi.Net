using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Net.Core.Extensions
{
    /// <summary>
    /// enum json converter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumConvert<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString().ToLower();

            foreach (T suit in Enum.GetValues(typeof(T)))
            {
                if (suit.ToString().ToLower() == value.ToString().ToLower())
                    return suit;
            }

            return 0;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
