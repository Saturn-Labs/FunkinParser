using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Data.Attributes;
using Funkin.Data.Versions.Latest;
using Funkin.Data.Versions.Latest.Chart;
using Funkin.Utils.Interfaces;
using NuGet.Versioning;

namespace Funkin.Data.Versions.v100.Chart
{
    [MetadataDescriptor("1.0.0", "[1.0.0,2.0.0)", MetadataType.Chart)]
    public class ChartData : ChartDataBase, ICloneable<ChartData>, IConvertible<Metadata, Latest.Chart.ChartData>
    {
        [JsonPropertyName("song")]
        public Chart Song { get; set; } = new();

        [JsonIgnore]
        public string DifficultyConvertKey { get; set; } = "normal";
        public int DifficultyConvertRating { get; set; } = 1;
        
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }
        
        public ChartData CloneTyped()
        {
            return new ChartData()
            {
                Song = Song.CloneTyped(),
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public (Metadata, Latest.Chart.ChartData) Convert()
        {
            var meta = new Metadata
            {
                SongName = Song.SongName,
                Divisions = 96,
                TimeFormat = TimeFormat.Milliseconds,
                Looped = false,
                Offsets = new Offsets(),
                TimeChanges = new []
                {
                    new TimeChange
                    {
                        TimeStamp = -1f,
                        TimeSignatureNumerator = 4,
                        TimeSignatureDenominator = 4,
                        BeatTime = 4,
                        BeatTuplets = new[] { 4, 4, 4, 4 },
                        BeatsPerMinute = Song.BeatsPerMinute
                    }
                },
                Variation = "default",
                PlayData = new PlayData
                {
                    Stage = Song.Stage,
                    Characters = new CharacterData(Song.Player, Song.Girlfriend, Song.Opponent),
                    NoteStyle = "funkin",
                    Difficulties = new[] { DifficultyConvertKey },
                    PreviewStart = 0,
                    PreviewEnd = 15000,
                    Ratings = new Dictionary<string, int>
                    {
                        { DifficultyConvertKey, DifficultyConvertRating }
                    }
                },
                ExtensionData = new Dictionary<string, JsonElement>(Song.ExtensionData ?? new Dictionary<string, JsonElement>())
            };
            
            var chart = new Latest.Chart.ChartData
            {
                Variation = "default",
                ScrollSpeed = new Dictionary<string, float>
                {
                    { DifficultyConvertKey, Song.Speed }
                },
                Events = Array.Empty<EventData>(),
                Notes = new Dictionary<string, NoteData[]>
                {
                    {
                        DifficultyConvertKey,
                        Song.Sections
                            .SelectMany(section => section.Notes)
                            .Select(note => note.Convert())
                            .Where(n => n is not null)
                            .OrderBy(n => n!.Time)
                            .ToArray()!
                    }
                },
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
            
            return (meta, chart);
        }

        public override string ToString()
        {
            return Song.ToString();
        }
    }
}