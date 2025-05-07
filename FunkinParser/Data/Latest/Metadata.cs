using System;
using System.Collections.Generic;
using System.Linq;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace Funkin.Data.Latest
{
    /**
     * Data containing information about a song.
     * It should contain all the data needed to display a song in the Freeplay menu, or to load the assets required to play its chart.
     * Data which is only necessary in-game should be stored in the ChartData.
     */
    [Serializable]
    public class Metadata : VersionStamp, ICloneable<Metadata>, IMetadata
    {
        public const string DefaultVersion = "2.2.4";
        public const string VersionRange = "[2.2.0,2.3.0)";
        
        [JsonProperty("songName")]
        public string SongName { get; set; } = "Unknown";
        
        [JsonProperty("artist")]
        public string Artist { get; set; } = "Unknown";

        [JsonProperty("charter")] 
        public string? Charter { get; set; }

        [JsonProperty("divisions")] 
        public int? Divisions { get; set; } = 96;

        [JsonProperty("looped")] 
        public bool Looped { get; set; }
        
        /**
         * Instrumental and vocal offsets.
         * Defaults to an empty Offsets object.
         */
        [JsonProperty("offsets")]
        public Offsets Offsets { get; set; } = new();
        
        /**
         * Data relating to the song's gameplay.
         */
        [JsonProperty("playData")]
        public PlayData PlayData { get; set; } = new();
        
        [JsonProperty("generatedBy")]
        public string GeneratedBy { get; set; }

        [JsonProperty("timeFormat")]
        [JsonConverter(typeof(SongTimeFormatConverter))]
        public TimeFormat TimeFormat { get; set; } = TimeFormat.Milliseconds;
        
        [JsonProperty("timeChanges")]
        public TimeChange[] TimeChanges { get; set; } = Array.Empty<TimeChange>();
        
        [JsonProperty("variation")]
        [JsonConverter(typeof(WriteIgnoreConverter))]
        public string Variation { get; set; } = "default";
        
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }

        public Metadata()
        {
            Version = NuGetVersion.Parse(DefaultVersion);
            GeneratedBy = $"FunkinParser - v{Version}";
        }

        public Metadata(string songName, string artist, string variation = "default") : this()
        {
            SongName = songName;
            Artist = artist;
            TimeFormat = TimeFormat.Milliseconds;
            Divisions = null;
            Offsets = new Offsets();
            TimeChanges = new[]
            {
                new TimeChange(0, 100)
            };
            Looped = false;
            PlayData = new PlayData
            {
                SongVariations = Array.Empty<string>(),
                Difficulties = Array.Empty<string>(),
                Characters = new CharacterData("bf", "gf", "dad"),
                Stage = "mainStage",
                NoteStyle = "funkin"
            };
            // Variation ID
            Variation = variation;
        }
        
        /**
         * Create a copy of this SongMetadata with the same information.
         * <returns>The cloned SongMetadata</returns>
         */
        public Metadata CloneTyped()
        {
            return new Metadata(SongName, Artist, Variation)
            {
                Version = Version,
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                Offsets = Offsets,
                TimeChanges = TimeChanges.Select(c => c.CloneTyped()).ToArray(),
                Looped = Looped,
                PlayData = PlayData,
                GeneratedBy = GeneratedBy
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        /**
         * Produces a string representation suitable for debugging.
         */
        public override string ToString()
        {
            return $"SongMetadata({SongName} by {Artist}, variation {Variation})";
        }
    }
}