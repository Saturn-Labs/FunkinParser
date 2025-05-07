using System;
using System.Collections.Generic;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funkin.Data.Latest
{
    /// <summary>
    /// Contains gameplay-related data for a song.
    /// This includes information about variations, difficulties, characters, and other gameplay-specific settings.
    /// </summary>
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PlayData : ICloneable<PlayData>
    {
        /// <summary>
        /// The variations of the song available for gameplay.
        /// </summary>
        [JsonProperty("songVariations")]
        public string[]? SongVariations { get; set; } = null;

        /// <summary>
        /// The difficulties available for the song.
        /// </summary>
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The characters involved in the song's gameplay.
        /// </summary>
        [JsonProperty("characters")]
        public CharacterData Characters { get; set; } = new();

        /// <summary>
        /// The stage where the song is played.
        /// </summary>
        [JsonProperty("stage")]
        public string Stage { get; set; } = "mainStage";

        /// <summary>
        /// The note style used in the song.
        /// </summary>
        [JsonProperty("noteStyle")]
        public string NoteStyle { get; set; } = "funkin";

        /// <summary>
        /// The ratings for the song, categorized by difficulty or other criteria.
        /// </summary>
        [JsonProperty("ratings")]
        public Dictionary<string, int> Ratings { get; set; } = new();

        /// <summary>
        /// The album associated with the song, if any.
        /// </summary>
        [JsonProperty("album")]
        public string? Album { get; set; }

        /// <summary>
        /// The start time for the audio preview in Freeplay.
        /// Defaults to 0 seconds in.
        /// </summary>
        /// <since>2.2.2</since>
        [JsonProperty("previewStart")]
        public int PreviewStart { get; set; }

        /// <summary>
        /// The end time for the audio preview in Freeplay.
        /// Defaults to 15 seconds in.
        /// </summary>
        /// <since>2.2.2</since>
        [JsonProperty("previewEnd")]
        public int PreviewEnd { get; set; } = 15000;

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }

        /// <summary>
        /// Creates a deep copy of this <see cref="PlayData"/> instance.
        /// </summary>
        /// <returns>A new <see cref="PlayData"/> instance with the same data.</returns>
        public PlayData CloneTyped()
        {
            return new PlayData
            {
                SongVariations = (string[]?)SongVariations?.Clone(),
                Difficulties = (string[])Difficulties.Clone(),
                Characters = Characters.CloneTyped(),
                Stage = Stage,
                NoteStyle = NoteStyle,
                Ratings = new Dictionary<string, int>(Ratings),
                Album = Album,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd,
                ExtensionData = new Dictionary<string, JToken>(ExtensionData ?? new Dictionary<string, JToken>()),
            };
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="PlayData"/> instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return CloneTyped();
        }
    }
}