﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Data.Versions.Latest;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.v210
{
    /// <summary>
    /// Contains gameplay-related data for a song.
    /// This includes information about variations, difficulties, characters, and other gameplay-specific settings.
    /// </summary>
    [Serializable]
    public class PlayData : ICloneable<PlayData>, IConvertible<Latest.PlayData>
    {
        /// <summary>
        /// The variations of the song available for gameplay.
        /// </summary>
        [JsonPropertyName("songVariations")]
        public string[]? SongVariations { get; set; } = null;

        /// <summary>
        /// The difficulties available for the song.
        /// </summary>
        [JsonPropertyName("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The characters involved in the song's gameplay.
        /// </summary>
        [JsonPropertyName("characters")]
        public CharacterData Characters { get; set; } = new();

        /// <summary>
        /// The stage where the song is played.
        /// </summary>
        [JsonPropertyName("stage")]
        public string Stage { get; set; } = "mainStage";

        /// <summary>
        /// The note skin used in the song.
        /// </summary>
        [JsonPropertyName("noteSkin")]
        public string NoteSkin { get; set; } = "funkin";

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

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
                NoteSkin = NoteSkin,
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
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

        public Latest.PlayData Convert()
        {
            return new Latest.PlayData
            {
                SongVariations = (string[]?)SongVariations?.Clone(),
                Difficulties = (string[])Difficulties.Clone(),
                Characters = Characters.CloneTyped(),
                Stage = Stage,
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
                // Renamed from NoteSkin to NoteStyle since v2.2.0
                NoteStyle = NoteSkin,
                // New stuff since v2.2.0
                Ratings = new Dictionary<string, int>(),
                Album = null,
                PreviewStart = 0,
                PreviewEnd = 15000
            };
        }
        
        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"SongPlayData[LEGACY:v2.1.0]([{string.Join(", ", SongVariations ?? Array.Empty<string>())}], [{string.Join(", ", Difficulties)}])";
        }
    }
}