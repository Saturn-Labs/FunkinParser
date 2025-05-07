using System;
using System.Reflection;
using System.Text.Json.Serialization;
using Funkin.Data.Attributes;
using Funkin.Data.Converters;
using NuGet.Versioning;

namespace Funkin.Utils.Interfaces
{
    [Serializable]
    public abstract class VersionStamp
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public NuGetVersion DefaultVersion
        {
            get
            {
                if (GetType().GetCustomAttribute<MetadataDescriptorAttribute>() is not { } attr)
                    return new NuGetVersion(1, 0, 0);
                return attr.Version;
            }
        }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        private NuGetVersion? _version = null;
        
        /// <summary>
        /// A semantic versioning for the data format.
        /// </summary>
        [JsonPropertyName("version")]
        [JsonConverter(typeof(VersionConverter))]
        public NuGetVersion Version {
            get => _version ?? DefaultVersion;
            set => _version = value;
        }

        protected VersionStamp()
        {
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