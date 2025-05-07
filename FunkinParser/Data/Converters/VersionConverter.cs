using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Versioning;

namespace Funkin.Data.Converters
{
    public class VersionConverter : JsonConverter<NuGetVersion>
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(NuGetVersion);
        public override NuGetVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var versionString = reader.GetString();
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

        public override void Write(Utf8JsonWriter writer, NuGetVersion value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToNormalizedString(), options);
        }
    }
}