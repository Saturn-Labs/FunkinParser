using System;
using System.Collections.Generic;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funkin.Data.Latest
{
    public class PlayData : ICloneable<PlayData>
    {
        /**
         * The variations this song has. The associated metadata files should exist.
         */
        [JsonProperty("songVariations")]
        public string[] SongVariations { get; set; } = Array.Empty<string>();
        
        /**
         * The difficulties contained in this song's chart file.
         */
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();
        
        /**
         * The characters used by this song.
         */
        [JsonProperty("characters")]
        public CharacterData Characters { get; set; } = new();
        
        /**
         * The stage used by this song.
         */
        [JsonProperty("stage")]
        public string Stage { get; set; } = string.Empty;
        
        /**
         * The note style used by this song.
         */
        [JsonProperty("noteStyle")]
        public string NoteStyle { get; set; } = string.Empty;

        /**
         * The difficulty ratings for this song as displayed in Freeplay.
         * Key is a difficulty ID.
         */
        [JsonProperty("ratings")]
        public Dictionary<string, int> Ratings { get; set; } = new();
        
        /**
         * The album ID for the album to display in Freeplay.
         * If `null`, display no album.
         */
        [JsonProperty("album")]
        public string? Album { get; set; }
        
        /**
         * The sticker pack for the song to use during transitions.
         * If `null`, display the character's sticker pack.
         */
        [JsonProperty("stickerPack")]
        public string? StickerPack { get; set; }

        /**
         * <summary>
         * The start time for the audio preview in Freeplay.
         * Defaults to 0 seconds in.
         * </summary>
         * <since>2.2.2</since>
         */
        [JsonProperty("previewStart")]
        public int PreviewStart { get; set; }
        
        /**
         * <summary>
         * The end time for the audio preview in Freeplay.
         * Defaults to 15 seconds in.
         * </summary>
         * <since>2.2.2</since>
         */
        [JsonProperty("previewEnd")]
        public int PreviewEnd { get; set; } = 15000;
        
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }
        
        public PlayData CloneTyped()
        {
            return new PlayData
            {
                SongVariations = (string[])SongVariations.Clone(),
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

        public object Clone()
        {
            return CloneTyped();
        }
    }
}