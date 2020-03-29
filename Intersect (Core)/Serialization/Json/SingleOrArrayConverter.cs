using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Serialization.Json
{

    public class SingleOrArrayConverter<T> : JsonConverter
    {

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var token = JToken.Load(reader);

            if (token == null)
            {
                return new List<T>();
            }

            return token.Type == JTokenType.Array ? token.ToObject<List<T>>() : new List<T> {token.ToObject<T>()};
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }

}
