using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.v100.Chart
{
    public class Section : ICloneable<Section>
    {
        [JsonPropertyName("sectionNotes")]
        [JsonConverter(typeof(LegacyNotesConverter))]
        public List<Note> Notes { get; set; } = new();
        
        [JsonPropertyName("sectionBeats")]
        public int SectionBeats { get; set; } = 4;
        
        [JsonPropertyName("mustHitSection")]
        public bool MustHitSection { get; set; } = true;
        
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }
        
        public Section CloneTyped()
        {
            return new Section
            {
                Notes = Notes.Select(n => n.CloneTyped()).ToList(),
                SectionBeats = SectionBeats,
                MustHitSection = MustHitSection,
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }
        
        public override string ToString()
        {
            return $"Section[LEGACY:1.0.0]({SectionBeats}, {MustHitSection}, {Notes.Count} notes)";
        }
    }
}