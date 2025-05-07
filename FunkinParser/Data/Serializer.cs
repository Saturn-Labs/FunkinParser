using System;
using System.Collections.Generic;
using System.Linq;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace Funkin.Data
{
    public static class Serializer
    {
        private static Dictionary<VersionRange, Type> _MetadataTypesForVersionRanges = new();

        static Serializer()
        {
            AddMetadataType<Latest.Metadata>(Latest.Metadata.VersionRange);
        }
        
        public static IMetadata? ParseMetadata(string metadataJsonText)
        {
            var obj = JObject.Parse(metadataJsonText);
            if (!obj.TryGetValue("version", out var versionObj) || versionObj.Value<string>() is not { } version)
                throw new FormatException("Metadata doesn't contain a 'version' string.");
            var type = _MetadataTypesForVersionRanges.FirstOrDefault(kv => kv.Key.Satisfies(NuGetVersion.Parse(version))).Value;
            if (type is null)
                throw new FormatException($"Couldn't find a proper metadata type for version '{version}'.");
            return JsonConvert.DeserializeObject(version, type) as IMetadata;
        }

        public static void AddMetadataType(string range, Type type) => AddMetadataType(VersionRange.Parse(range), type);
        public static void AddMetadataType(VersionRange range, Type type)
        {
            _MetadataTypesForVersionRanges[range] = type;
        }
        
        public static void AddMetadataType<T>(string range) where T : IMetadata => AddMetadataType<T>(VersionRange.Parse(range));
        public static void AddMetadataType<T>(VersionRange range) where T : IMetadata => AddMetadataType(range, typeof(T));
    }
}