using Jtext103.CFET2.Core.Sample;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtext103.CFET2.WebsocketEvent
{
    class SampleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ISample));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(Status<object>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(ISample));
        }
    }
}
