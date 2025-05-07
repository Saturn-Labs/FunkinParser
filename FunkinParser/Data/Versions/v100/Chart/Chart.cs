using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.v100.Chart
{
    public class Chart : ICloneable<Chart>
    {
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, Chart instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "player3" => false,
                _ => true
            };
        }
        
        [JsonPropertyName("song")]
        public string SongName { get; set; } = string.Empty;
        
        [JsonPropertyName("bpm")]
        public float BeatsPerMinute { get; set; } = 120.0f;

        [JsonPropertyName("speed")]
        public float Speed { get; set; } = 2.0f;
        
        [JsonPropertyName("offset")]
        public float Offset { get; set; } = 0.0f;
        
        [JsonPropertyName("needsVoices")]
        public bool NeedsVoices { get; set; } = true;
        
        [JsonPropertyName("stage")]
        public string Stage { get; set; } = "stage";
        
        [JsonPropertyName("player1")]
        public string Player { get; set; } = "bf";
        
        [JsonPropertyName("player2")]
        public string Opponent { get; set; } = "dad";

        [JsonPropertyName("player3")]
        [PredicateIgnore]
        public string Player3
        {
            get => Girlfriend;
            set => Girlfriend = value;
        }
        
        [JsonPropertyName("gfVersion")]
        public string Girlfriend { get; set; } = "gf";
        
        [JsonPropertyName("notes")]
        public List<Section> Sections { get; set; } = new();

        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }
        
        public Chart CloneTyped()
        {
            return new Chart
            {
                SongName = SongName,
                BeatsPerMinute = BeatsPerMinute,
                Speed = Speed,
                Offset = Offset,
                NeedsVoices = NeedsVoices,
                Stage = Stage,
                Player = Player,
                Opponent = Opponent,
                Girlfriend = Girlfriend,
                Sections = Sections.Select(s => s.CloneTyped()).ToList(),
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public override string ToString()
        {
            return @$"SongChart[LEGACY:1.0.0](""{SongName}"", {BeatsPerMinute}bpm, {Speed}f, {Offset}f, {NeedsVoices}, ""{Stage}"", ""{Player}"", ""{Opponent}"", ""{Girlfriend}"", {Sections.Count} sections)";
        }
    }
}