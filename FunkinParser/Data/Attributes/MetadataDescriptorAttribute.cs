using System;
using NuGet.Versioning;

namespace Funkin.Data.Attributes
{
    public enum MetadataType
    {
        Metadata,
        Chart
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MetadataDescriptorAttribute : Attribute
    {
        public NuGetVersion Version { get; }
        public VersionRange VersionRange { get; }
        public MetadataType Type { get; }
        
        public MetadataDescriptorAttribute(string version, string versionRange, MetadataType type = MetadataType.Metadata)
        {
            Version = NuGetVersion.Parse(version);
            VersionRange = VersionRange.Parse(versionRange);
            Type = type;
        }
    }
}