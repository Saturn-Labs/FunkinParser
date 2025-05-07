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
    /// <summary>
    /// Data containing information about a song.
    /// It should contain all the data needed to display a song in the Freeplay menu, or to load the assets required to play its chart.
    /// Data which is only necessary in-game should be stored in the ChartData.
    /// </summary>
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Metadata : VersionStamp, ICloneable<Metadata>, IMetadata
    {
        /// <summary>
        /// The default version of the metadata.
        /// </summary>
        public static readonly string DefaultVersion = "2.2.4";

        /// <summary>
        /// The version range supported by this metadata.
        /// </summary>
        public static readonly string VersionRange = "[2.2.0,2.3.0)";

        /// <summary>
        /// The name of the song.
        /// </summary>
        [JsonProperty("songName")]
        public string SongName { get; set; } = "Unknown";

        /// <summary>
        /// The artist of the song.
        /// </summary>
        [JsonProperty("artist")]
        public string Artist { get; set; } = "Unknown";

        /// <summary>
        /// The charter of the song.
        /// </summary>
        [JsonProperty("charter")]
        public string? Charter { get; set; }

        /// <summary>
        /// The number of divisions in the song.
        /// </summary>
        [JsonProperty("divisions")]
        public int? Divisions { get; set; } = 96;

        /// <summary>
        /// Indicates whether the song is looped.
        /// </summary>
        [JsonProperty("looped")]
        public bool Looped { get; set; }

        /// <summary>
        /// Instrumental and vocal offsets.
        /// Defaults to an empty Offsets object.
        /// </summary>
        [JsonProperty("offsets")]
        public Offsets Offsets { get; set; } = new();

        /// <summary>
        /// Data relating to the song's gameplay.
        /// </summary>
        [JsonProperty("playData")]
        public PlayData PlayData { get; set; } = new();

        /// <summary>
        /// The generator of this metadata.
        /// </summary>
        [JsonProperty("generatedBy")]
        public string GeneratedBy { get; set; }

        /// <summary>
        /// The time format used in the metadata.
        /// </summary>
        [JsonProperty("timeFormat")]
        [JsonConverter(typeof(TimeFormatConverter))]
        public TimeFormat TimeFormat { get; set; } = TimeFormat.Milliseconds;

        /// <summary>
        /// The time changes in the song.
        /// </summary>
        [JsonProperty("timeChanges")]
        public TimeChange[] TimeChanges { get; set; } = Array.Empty<TimeChange>();

        /// <summary>
        /// The variation of the song.
        /// </summary>
        [JsonProperty("variation")]
        [JsonConverter(typeof(WriteIgnoreConverter))]
        public string Variation { get; set; } = "default";

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of the Metadata class with default values.
        /// </summary>
        public Metadata()
        {
            Version = NuGetVersion.Parse(DefaultVersion);
            GeneratedBy = $"FunkinParser - v{Version}";
        }

        /// <summary>
        /// Initializes a new instance of the Metadata class with specified values.
        /// </summary>
        /// <param name="songName">The name of the song.</param>
        /// <param name="artist">The artist of the song.</param>
        /// <param name="variation">The variation of the song.</param>
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
            Variation = variation;
        }

        /// <summary>
        /// Creates a copy of this Metadata with the same information.
        /// </summary>
        /// <returns>The cloned Metadata.</returns>
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

        /// <summary>
        /// Creates a copy of this Metadata.
        /// </summary>
        /// <returns>The cloned Metadata.</returns>
        public object Clone()
        {
            return CloneTyped();
        }

        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        /// <returns>A string representation of the Metadata.</returns>
        public override string ToString()
        {
            return $"SongMetadata({SongName} by {Artist}, variation {Variation})";
        }
    }
}