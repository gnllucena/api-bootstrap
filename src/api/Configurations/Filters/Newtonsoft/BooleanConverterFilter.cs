using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace API.Configurations.Filters.Newtonsoft 
{
    public class BooleanConverterFilter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString().ToLower().Trim();

            switch (value)
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    return true;
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var binary = value == null ? false : Convert.ToBoolean(value);

            writer.WriteValue(Convert.ToBoolean(binary));
        }
    }
}