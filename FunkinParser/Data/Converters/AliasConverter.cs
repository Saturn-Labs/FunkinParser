using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Funkin.Data.Converters
{
    public class AliasConverter : JsonConverter
    {
        private readonly string[] _aliasNames;
        private readonly string _mainName;

        public AliasConverter(string mainName, params string[] aliasNames)
        {
            _mainName = mainName;
            _aliasNames = aliasNames.Prepend(mainName).ToArray();
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanConvert(Type objectType) => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }
            var jObject = new JObject
            {
                [_mainName] = JToken.FromObject(value, serializer)
            };
            jObject.WriteTo(writer);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (jObject.TryGetValue(_mainName, out var token))
            {
                return token.ToObject(objectType, serializer);
            }

            return _aliasNames.Any(name => jObject.TryGetValue(name, out token)) ? token?.ToObject(objectType, serializer) : null;
        }
    }
}