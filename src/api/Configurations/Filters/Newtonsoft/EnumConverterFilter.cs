using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace API.Configurations.Filters.Newtonsoft 
{
    public class EnumConverterFilter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(!Enum.IsDefined(objectType, reader.Value))
            {
                return null;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}