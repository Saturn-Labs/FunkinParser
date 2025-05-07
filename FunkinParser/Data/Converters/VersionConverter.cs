using System;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Data.Converters
{
    public class VersionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
                return;
            writer.WriteValue(value.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var versionString = reader.Value as string;
            try
            {
                return versionString is null ? null : NuGetVersion.Parse(versionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't parse version '{versionString}'.");
            }
            return null;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(NuGetVersion);
    }
}