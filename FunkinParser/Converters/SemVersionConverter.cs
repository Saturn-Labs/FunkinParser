using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace Funkin.Converters
{
    public class SemVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(NuGetVersion);
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("version");
            writer.WriteValue(value is null ? "2.0.0" : value.ToString());
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var str = reader.Value as string;
            return str is null ? new NuGetVersion(2, 0, 0) : NuGetVersion.Parse(str);
        }
    }
}