using System;
using Funkin.Data.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Utils.Interfaces
{
    [Serializable]
    public abstract class VersionStamp
    {
        /**
         * A semantic versioning for the data format.
         */
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        public NuGetVersion Version { get; protected set; }

        protected VersionStamp()
        {
            Version = new NuGetVersion(0, 0, 1);
        }
        
        protected VersionStamp(NuGetVersion version)
        {
            Version = version;
        }

        protected VersionStamp(string version)
        {
            Version = NuGetVersion.Parse(version);
        }
    }
}