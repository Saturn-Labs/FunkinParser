using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Converters;
using Funkin.Core.Data.v20X;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NuGet.Versioning;

namespace Funkin.Core.Data.v10X
{
    public class SongChartData : VersioningData, IData, IVersionConvertible<v20X.SongChartData>, ICloneable<SongChartData>
    {
        [JsonProperty("song")]
        public SongData Song { get; set; } = new SongData();
        
        public SongChartData()
        {
            Version = NuGetVersion.Parse("1.0.0");
        }
        
        private static readonly PropertyInfo[] PropertiesCache = typeof(v20X.SongChartData).GetProperties();
        public object? this[string key] => GetValue(key);

        public object? GetValue(string key)
        {
            var prop = PropertiesCache.FirstOrDefault(p =>
            {
                if (p.Name == key)
                    return true;
                if (!(p.GetCustomAttribute<JsonPropertyAttribute>() is {} attr))
                    return false;
                return attr.PropertyName == key;
            });
            return prop?.GetValue(this);
        }

        public T GetValue<T>(string key)
        {
            if (!(GetValue(key) is T value))
                throw new InvalidCastException($"Failed to cast key '{key}' value to type '{typeof(T).Name}'.");
            return value;
        }

        public bool HasValue(string key)
        {
            return PropertiesCache.FirstOrDefault(p =>
                {
                    if (p.Name == key)
                        return true;
                    if (!(p.GetCustomAttribute<JsonPropertyAttribute>() is { } attr))
                        return false;
                    return attr.PropertyName == key;
                }
            ) is {};
        }

        public v20X.SongChartData Convert()
        {
            var notes = Song.Notes.SelectMany(n => n.SectionNotes).Select(n => new SongNoteData()
            {
                Time = n.Time,
                Data = n.StrumType,
                Length = n.Length,
                Params = n.CustomData is null ? Array.Empty<NoteParamData>() : new[]
                {
                    new NoteParamData("param1", n.CustomData)
                },
                Kind = "default"
            }).GroupBy(o => "normal").ToDictionary(o => o.Key, o => o.ToArray());
            return new v20X.SongChartData()
            {
                Events = Array.Empty<SongEventData>(),
                GeneratedBy = "Conversion on FunkinParser made by ryd3v.",
                Notes = notes,
                ScrollSpeed = new Dictionary<string, float>
                {
                    { "normal", Song.Speed }
                },
                Variation = "default"
            };
        }

        public SongChartData Clone()
        {
            return new SongChartData()
            {
                Version = Version,
                Song = Song.Clone(),
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public class SongData : ICloneable<SongData>
    {
        [JsonProperty("song")]
        public string Song { get; set; } = string.Empty;
        [JsonProperty("bpm")] 
        public float Bpm { get; set; } = 0;
        [JsonProperty("speed")]
        public float Speed { get; set; } = 0;
        [JsonProperty("stage")]
        public string Stage { get; set; } = string.Empty;
        [JsonProperty("player1")]
        public string Player1 { get; set; } = "bf";
        [JsonProperty("player2")]
        public string Player2 { get; set; } = "dad";
        [JsonProperty("gfVersion")]
        public string GfVersion { get; set; } = string.Empty;
        [JsonProperty("player3")]
        public string Player3
        {
            get => GfVersion;
            set => GfVersion = value;
        }
        [JsonProperty("needsVoices")]
        public bool NeedsVoices { get; set; } = false;
        [JsonProperty("offset")]
        public float Offset { get; set; } = 0;
        [JsonProperty("notes")]
        public Section[] Notes { get; set; } = Array.Empty<Section>();

        public SongData Clone()
        {
            return new SongData()
            {
                Song = Song,
                Bpm = Bpm,
                Speed = Speed,
                Stage = Stage,
                Player1 = Player1,
                Player2 = Player2,
                GfVersion = GfVersion,
                Player3 = Player3,
                NeedsVoices = NeedsVoices,
                Offset = Offset,
                Notes = Notes.Select(n => n.Clone()).ToArray()
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public class Section : ICloneable<Section>
    {
        [JsonProperty("sectionNotes")]
        [JsonConverter(typeof(LegacyNoteConverter))]
        public Note[] SectionNotes { get; set; } = Array.Empty<Note>();
        [JsonProperty("sectionBeats")] 
        public int SectionBeats { get; set; } = 4;
        [JsonProperty("mustHitSection")]
        public bool MustHitSection { get; set; } = false;

        public Section Clone()
        {
            return new Section()
            {
                SectionBeats = SectionBeats,
                MustHitSection = MustHitSection,
                SectionNotes = SectionNotes.Select(n => n.Clone()).ToArray()
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
    
    public class Note : ICloneable<Note>
    {
        public float Time { get; set; } = 0.0f;
        public int StrumType { get; set; } = 0;
        public float Length { get; set; } = 0.0f;
        public object? CustomData { get; set; } = null;
        public Note Clone()
        {
            return new Note()
            {
                Time = Time,
                StrumType = StrumType,
                Length = Length,
                CustomData = CustomData
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}