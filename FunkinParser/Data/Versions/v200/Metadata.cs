using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Data.Converters;
using Funkin.Data.Versions.Latest;
using Funkin.Utils.Interfaces;
using NuGet.Versioning;

namespace Funkin.Data.Versions.v200
{
    /// <summary>
    /// Data containing information about a song.
    /// It should contain all the data needed to display a song in the Freeplay menu, or to load the assets required to play its chart.
    /// Data which is only necessary in-game should be stored in the ChartData.
    /// </summary>
    [Serializable]
    [MetadataDescriptor("2.0.0", "[2.0.0,2.1.0)")]
    public class Metadata : MetadataBase, ICloneable<Metadata>, IConvertible<Latest.Metadata>
    {
        /// <summary>
        /// Determines whether the specified property should be serialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be serialized.</returns>
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, Metadata instance, object? value)
        {
            return propertyInfo.Name switch
            {
                _ => true
            };
        }
        
        /// <summary>
        /// Determines whether the specified property should be deserialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be deserialized.</returns>
        public static bool ShouldDeserialize(JsonPropertyInfo propertyInfo, Metadata instance, object? value)
        {
            return propertyInfo.Name switch
            {
                _ => true
            };
        }

        /// <summary>
        /// The name of the song.
        /// </summary>
        [JsonPropertyName("songName")]
        public string SongName { get; set; } = "Unknown";

        /// <summary>
        /// The artist of the song.
        /// </summary>
        [JsonPropertyName("artist")]
        public string Artist { get; set; } = "Unknown";

        /// <summary>
        /// The charter of the song.
        /// </summary>
        [JsonPropertyName("charter")]
        public string? Charter { get; set; }

        /// <summary>
        /// The number of divisions in the song.
        /// </summary>
        [JsonPropertyName("divisions")]
        public int? Divisions { get; set; } = 96;

        /// <summary>
        /// Indicates whether the song is looped.
        /// </summary>
        [JsonPropertyName("looped")]
        public bool Looped { get; set; }

        /// <summary>
        /// Data relating to the song's gameplay.
        /// </summary>
        [JsonPropertyName("playData")]
        public PlayData PlayData { get; set; } = new();

        /// <summary>
        /// The generator of this metadata.
        /// </summary>
        [JsonPropertyName("generatedBy")]
        public string GeneratedBy { get; set; }

        /// <summary>
        /// The time format used in the metadata.
        /// </summary>
        [JsonPropertyName("timeFormat")]
        [JsonConverter(typeof(TimeFormatConverter))]
        public TimeFormat TimeFormat { get; set; } = TimeFormat.Milliseconds;

        /// <summary>
        /// The time changes in the song.
        /// </summary>
        [JsonPropertyName("timeChanges")]
        public TimeChange[] TimeChanges { get; set; } = Array.Empty<TimeChange>();

        /// <summary>
        /// The variation of the song.
        /// </summary>
        [JsonPropertyName("variation")]
        public string Variation { get; set; } = "default";

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of the Metadata class with default values.
        /// </summary>
        public Metadata()
        {
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
            TimeChanges = new[]
            {
                new TimeChange(0, 100)
            };
            Looped = false;
            PlayData = new PlayData
            {
                SongVariations = Array.Empty<string>(),
                Difficulties = Array.Empty<string>(),
                PlayableChars = new Dictionary<string, PlayableChar>(),
                Stage = "mainStage",
                NoteSkin = "funkin"
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
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                TimeChanges = TimeChanges.Select(c => c.CloneTyped()).ToArray(),
                Looped = Looped,
                PlayData = PlayData.CloneTyped(),
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
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

        public Latest.Metadata? Convert()
        {
            if (PlayData.Convert() is not { } playData)
            {
                return null;
            }
            return new Latest.Metadata(SongName, Artist, Variation)
            {
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                TimeChanges = TimeChanges.Select(c => c.CloneTyped()).ToArray(),
                Looped = Looped,
                // Converted PlayData to Latest
                PlayData = playData!,
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
            };
        }

        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        /// <returns>A string representation of the Metadata.</returns>
        public override string ToString()
        {
            return $"SongMetadata[LEGACY:v2.0.0]({SongName} by {Artist}, variation {Variation})";
        }
    }
}