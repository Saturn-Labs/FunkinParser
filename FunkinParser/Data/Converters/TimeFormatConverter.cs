using System;
using Funkin.Data.Latest;
using Newtonsoft.Json;

namespace Funkin.Data.Converters
{
    public class TimeFormatConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is not TimeFormat format)
                throw new JsonSerializationException("Invalid TimeFormat value.");

            var strValue = format switch
            {
                TimeFormat.Milliseconds => "ms",
                TimeFormat.Float => "float",
                TimeFormat.Ticks => "ticks",
                _ => throw new JsonSerializationException("Unknown TimeFormat value.")
            };
            writer.WriteValue(strValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException("Expected string value for TimeFormat.");
            
            var value = (reader.Value as string)?.ToLowerInvariant();
            return value switch
            {
                "ms" => TimeFormat.Milliseconds,
                "float" => TimeFormat.Float,
                "ticks" => TimeFormat.Ticks,
                _ => throw new JsonSerializationException($"Unknown TimeFormat: '{value}'")
            };
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(TimeFormat);
    }
}