using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Data.Versions.Latest;

namespace Funkin.Data.Converters
{
    public class TimeFormatConverter : JsonConverter<TimeFormat>
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TimeFormat);
        public override TimeFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string value for TimeFormat.");
            
            var value = reader.GetString()?.ToLowerInvariant();
            return value switch
            {
                "ms" => TimeFormat.Milliseconds,
                "float" => TimeFormat.Float,
                "ticks" => TimeFormat.Ticks,
                _ => throw new JsonException($"Unknown TimeFormat: '{value}'")
            };
        }

        public override void Write(Utf8JsonWriter writer, TimeFormat value, JsonSerializerOptions options)
        {
            var strValue = value switch
            {
                TimeFormat.Milliseconds => "ms",
                TimeFormat.Float => "float",
                TimeFormat.Ticks => "ticks",
                _ => throw new JsonException("Unknown TimeFormat value.")
            };
            JsonSerializer.Serialize(writer, strValue, options);
        }
    }
}