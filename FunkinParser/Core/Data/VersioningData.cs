using Funkin.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Core.Data
{
    public class VersioningData
    {
        [JsonConverter(typeof(SemVersionConverter))]
        public NuGetVersion Version { get; set; } = NuGetVersion.Parse("2.2.4");
    }
}