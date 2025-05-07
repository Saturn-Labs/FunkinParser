using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest.Chart
{
    [MetadataDescriptor("2.0.0", "[2.0.0,2.1.0)", MetadataType.Chart)]
    public class ChartData : ChartDataBase, ICloneable<ChartData>
    {
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, ChartData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "variation" => false,
                _ => true
            };
        }
        
        [JsonPropertyName("scrollSpeed")]
        public Dictionary<string, float> ScrollSpeed { get; set; } = new();
        
        [JsonPropertyName("events")]
        public EventData[] Events { get; set; } = Array.Empty<EventData>();
        
        [JsonPropertyName("notes")]
        public Dictionary<string, NoteData[]> Notes { get; set; } = new();
        
        [JsonPropertyName("generatedBy")]
        public string GeneratedBy { get; set; }
        
        [JsonPropertyName("variation")]
        [PredicateIgnore]
        public string Variation { get; set; } = "default";

        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }
        
        public ChartData()
        {
            GeneratedBy = $"FunkinParser - v{Version}";
        }

        public ChartData(Dictionary<string, float> scrollSpeed, EventData[] events, Dictionary<string, NoteData[]> notes) : this()
        {
            Events = events;
            Notes = notes;
            ScrollSpeed = scrollSpeed;
        }
        
        public ChartData CloneTyped()
        {
            return new ChartData(new Dictionary<string, float>(ScrollSpeed), Events.Select(e => e.CloneTyped()).ToArray(),
                Notes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(n => n.CloneTyped()).ToArray()))
            {
                Variation = Variation,
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public override string ToString()
        {
            return $"SongChartData({Events.Length} events, {Notes.Count} difficulties, {GeneratedBy})";
        }
    }
}