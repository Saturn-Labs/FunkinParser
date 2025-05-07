using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Data.ContractResolvers;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace Funkin.Data
{
    public static class Converter
    {
        private static Dictionary<VersionRange, Type> _MetadataTypesForVersionRanges = new();
        private static readonly JsonSerializer _MetadataSerializer = new()
        {
            ContractResolver = new SelectableJsonIgnoreContractResolver()
        };
        
        static Converter()
        {
            AddMetadataType<Latest.Metadata>(Latest.Metadata.VersionRange);
        }

        public static IMetadata? DeserializeMetadata(JObject obj)
        {
            if (!obj.TryGetValue("version", out var versionObj) || versionObj.Value<string>() is not { } version)
                throw new FormatException("Metadata doesn't contain a 'version' string.");
            var type = _MetadataTypesForVersionRanges.FirstOrDefault(kv => kv.Key.Satisfies(NuGetVersion.Parse(version))).Value;
            if (type is null)
                throw new FormatException($"Couldn't find a proper metadata type for version '{version}'.");
            return obj.ToObject(type, _MetadataSerializer) as IMetadata;
        }
        
        public static IMetadata? DeserializeMetadata(string metadataJsonText)
        {
            var obj = JObject.Parse(metadataJsonText);
            return DeserializeMetadata(obj);
        }

        public static T? DeserializeMetadata<T>(string metadataJsonText) where T : class, IMetadata
        {
            if (typeof(T).GetField("VersionRange", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) is not {} versionRangeField)
                return null;
            var versionRangeString = versionRangeField.GetValue(null) as string;
            if (string.IsNullOrWhiteSpace(versionRangeString))
                return null;
            var versionRange = VersionRange.Parse(versionRangeString);
            var obj = JObject.Parse(metadataJsonText);
            if (!obj.TryGetValue("version", out var versionObj) || versionObj.Value<string>() is not { } version)
                throw new FormatException("Metadata doesn't contain a 'version' string.");
            if (!versionRange.Satisfies(NuGetVersion.Parse(version)))
                throw new FormatException($"Couldn't match type 'T' for version '{version}'.");
            return DeserializeMetadata(obj) as T;
        }

        public static string SerializeMetadata(IMetadata? metadata, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(metadata, formatting, new JsonSerializerSettings
            {
                ContractResolver = new SelectableJsonIgnoreContractResolver()
            });
        }

        public static void AddMetadataType(string range, Type type) => AddMetadataType(VersionRange.Parse(range), type);
        public static void AddMetadataType(VersionRange range, Type type)
        {
            _MetadataTypesForVersionRanges[range] = type;
        }
        
        public static void AddMetadataType<T>(string range) where T : class, IMetadata => AddMetadataType<T>(VersionRange.Parse(range));
        public static void AddMetadataType<T>(VersionRange range) where T : class, IMetadata => AddMetadataType(range, typeof(T));
    }
}