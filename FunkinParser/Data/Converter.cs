using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;
using NuGet.Versioning;

namespace Funkin.Data
{
    public static class Converter
    {
        private static readonly Dictionary<VersionRange, Type> _MetadataTypesForVersionRanges = new();
        private static readonly Dictionary<VersionRange, Type> _ChartDataTypesForVersionRanges = new();
        private static readonly JsonSerializerOptions _SerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            {
                Modifiers =
                {
                    (ti) =>
                    {
                        var ignoredProperties = new List<(PropertyInfo PropertyInfo, JsonPropertyInfo JsonPropertyInfo)>(); 
                        foreach (var property in ti.Properties)
                        {
                            var propInfo = property.AttributeProvider as PropertyInfo;
                            var ignoreAttribute = propInfo?.GetCustomAttribute<PredicateIgnoreAttribute>();
                            if (ignoreAttribute is null)
                                continue;
                            ignoredProperties.Add((propInfo!, property));
                        }

                        foreach (var (info, jsonInfo) in ignoredProperties)
                        {
                            var declaringType = info.DeclaringType;
                            var shouldSerializeFn = declaringType!.GetMethod("ShouldSerialize", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                            var shouldDeserializeFn = declaringType!.GetMethod("ShouldDeserialize", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                            jsonInfo.ShouldSerialize = (parent, value) => shouldSerializeFn?.Invoke(null, new[] { jsonInfo, parent, value }) as bool? ?? true;
                            if (shouldDeserializeFn is not null)
                            {
                                var oldSet = jsonInfo.Set;
                                jsonInfo.Set = (o, value) =>
                                {
                                    var shouldDeserialize = shouldDeserializeFn.Invoke(null, new[] { jsonInfo, o, value }) as bool? ?? true;
                                    if (shouldDeserialize)
                                        oldSet?.Invoke(o, value);
                                };
                            }
                        }
                    }
                }
            }
        };
        
        static Converter()
        {
            var metadataTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.GetCustomAttribute<MetadataDescriptorAttribute>() is not null);
            foreach (var type in metadataTypes)
            {
                var metadataAttr = type.GetCustomAttribute<MetadataDescriptorAttribute>();
                if (metadataAttr is null)
                    continue;
                switch (metadataAttr.Type)
                {
                    case MetadataType.Metadata:
                        AddMetadataType(metadataAttr.VersionRange, type);
                        break;
                    case MetadataType.Chart:
                        AddChartDataType(metadataAttr.VersionRange, type);
                        break;
                }
            }
        }

        public static MetadataBase? DeserializeMetadata(JsonNode obj)
        {
            var versionObj = obj["version"];
            if (versionObj?.GetValue<string>() is not { } version)
                throw new FormatException("Metadata doesn't contain a 'version' string.");
            var type = _MetadataTypesForVersionRanges.FirstOrDefault(kv => kv.Key.Satisfies(NuGetVersion.Parse(version))).Value;
            if (type is null)
                throw new FormatException($"Couldn't find a proper metadata type for version '{version}'.");
            return obj.Deserialize(type, _SerializerOptions) as MetadataBase;
        }
        
        public static MetadataBase? DeserializeMetadata(string metadataJsonText)
        {
            var obj = JsonNode.Parse(metadataJsonText);
            if (obj is null)
                throw new FormatException("Couldn't parse metadata JSON.");
            return DeserializeMetadata(obj);
        }

        public static T? DeserializeMetadata<T>(string metadataJsonText) where T : MetadataBase
        {
            if (typeof(T).GetCustomAttribute<MetadataDescriptorAttribute>() is not { } attr)
                return null;
            var versionRange = attr.VersionRange;
            var obj = JsonNode.Parse(metadataJsonText);
            if (obj is null)
                throw new FormatException("Couldn't parse metadata JSON.");
            
            var versionObj = obj["version"];
            if (versionObj?.GetValue<string>() is not { } version)
                throw new FormatException("Metadata doesn't contain a 'version' string.");
            if (!versionRange.Satisfies(NuGetVersion.Parse(version)))
                throw new FormatException($"Couldn't match type 'T' for version '{version}'.");
            return DeserializeMetadata(obj) as T;
        }

        public static ChartDataBase? DeserializeChartData(JsonNode obj)
        {
            var versionObj = obj["version"];
            if (versionObj?.GetValue<string>() is not { } version)
                version = "1.0.0";
            var type = _ChartDataTypesForVersionRanges.FirstOrDefault(kv => kv.Key.Satisfies(NuGetVersion.Parse(version))).Value;
            if (type is null)
                throw new FormatException($"Couldn't find a proper chart data type for version '{version}'.");
            return obj.Deserialize(type, _SerializerOptions) as ChartDataBase;
        }
        
        public static ChartDataBase? DeserializeChartData(string chartDataJsonText)
        {
            var obj = JsonNode.Parse(chartDataJsonText);
            if (obj is null)
                throw new FormatException("Couldn't parse metadata JSON.");
            return DeserializeChartData(obj);
        }
        
        public static T? DeserializeChartData<T>(string chartDataJsonText) where T : ChartDataBase
        {
            if (typeof(T).GetCustomAttribute<MetadataDescriptorAttribute>() is not { } attr)
                return null;
            var versionRange = attr.VersionRange;
            var obj = JsonNode.Parse(chartDataJsonText);
            if (obj is null)
                throw new FormatException("Couldn't parse metadata JSON.");
            
            var versionObj = obj["version"];
            if (versionObj?.GetValue<string>() is not { } version)
                version = "1.0.0";
            if (!versionRange.Satisfies(NuGetVersion.Parse(version)))
                throw new FormatException($"Couldn't match type 'T' for version '{version}'.");
            return DeserializeChartData(obj) as T;
        }

        public static string Serialize<T>(T? data, bool indented = true) where T : VersionStamp
        {
            return JsonSerializer.Serialize(data, data?.GetType() ?? typeof(T), new JsonSerializerOptions(_SerializerOptions)
            {
                WriteIndented = indented
            });
        }

        public static void AddMetadataType(string range, Type type) => AddMetadataType(VersionRange.Parse(range), type);
        public static void AddMetadataType(VersionRange range, Type type)
        {
            _MetadataTypesForVersionRanges[range] = type;
        }
        
        public static void AddMetadataType<T>(string range) where T : MetadataBase => AddMetadataType<T>(VersionRange.Parse(range));
        public static void AddMetadataType<T>(VersionRange range) where T : MetadataBase => AddMetadataType(range, typeof(T));
        
        public static void AddChartDataType(string range, Type type) => AddChartDataType(VersionRange.Parse(range), type);
        public static void AddChartDataType(VersionRange range, Type type)
        {
            _ChartDataTypesForVersionRanges[range] = type;
        }
        
        public static void AddChartDataType<T>(string range) where T : ChartDataBase => AddChartDataType<T>(VersionRange.Parse(range));
        public static void AddChartDataType<T>(VersionRange range) where T : ChartDataBase => AddChartDataType(range, typeof(T));
    }
}