using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Core.Data.v21X
{
    public class SongData : VersioningData, IData, ICloneable<SongData>, IVersionConvertible<Latest.SongData>
    {
        public static readonly NuGetVersion DefaultVersion = new("2.1.0");
        
        [JsonProperty("songName")]
        public string SongName { get; set; } = "Unknown";
        [JsonProperty("artist")]
        public string Artist { get; set; } = "Unknown";
        [JsonProperty("charter")]
        public string? Charter { get; set; } = null;
        [JsonProperty("divisions")]
        public int Divisions { get; set; } = 96;
        [JsonProperty("looped")]
        public bool Looped { get; set; } = false;
        [JsonProperty("playData")]
        public SongPlayData? PlayData { get; set; } = null;
        [JsonProperty("generatedBy")]
        public string GeneratedBy { get; set; } = "Unknown";
        [JsonProperty("timeFormat")] 
        public string TimeFormat { get; set; } = "ms";
        [JsonProperty("timeChanges")]
        public SongTimeChange[] TimeChanges { get; set; } = Array.Empty<SongTimeChange>();
        [JsonConverter(typeof(WriteIgnore))]
        public string Variation { get; set; } = "default";
        
        public SongData()
        {
            Version = DefaultVersion;
        }
        
        public SongData(string songName, string artist, string variation) : this()
        {
            SongName = songName;
            Artist = artist;
            Variation = variation;
        }
        
        public object Clone()
        {
            var res = new SongData
            {
                Version = Version,
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                TimeChanges = TimeChanges.Select(t => (SongTimeChange)t.Clone()).ToArray(),
                Looped = Looped,
                PlayData = PlayData?.Clone() as SongPlayData ?? new SongPlayData(),
                GeneratedBy = GeneratedBy
            };
            return res;
        }
        
        SongData ICloneable<SongData>.Clone()
        {
            return (SongData)Clone();
        }

        private static readonly PropertyInfo[] PropertiesCache = typeof(SongData).GetProperties();
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

        public Latest.SongData Convert()
        {
            return new Latest.SongData(SongName, Artist, Variation)
            {
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                TimeChanges = TimeChanges.Select(t => t.Convert()).ToArray(),
                Looped = Looped,
                PlayData = PlayData?.Convert() ?? new Latest.SongPlayData(),
                GeneratedBy = GeneratedBy
            };
        }

        public override string ToString()
        {
            return $"SongMetadata[LEGACY:v2.1.0]({SongName} by {Artist}, variation {Variation})";
        }
    }

    public class SongTimeChange : ICloneable<SongTimeChange>, IVersionConvertible<Latest.SongTimeChange>
    {
        [JsonProperty("timeStamp")]
        [JsonConverter(typeof(WriteIgnore))]
        public float TimeStamp { get; set; } = 0f;
        public float t { get => TimeStamp; set => TimeStamp = value; }
        
        [JsonProperty("beatTime")]
        [JsonConverter(typeof(WriteIgnore))]
        public float? BeatTime { get; set; } = 0f;
        public float? b { get => BeatTime; set => BeatTime = value; }
        
        [JsonProperty("bpm")] 
        public float Bpm { get; set; } = 0f;
        
        [JsonProperty("timeSignatureNum")]
        [JsonConverter(typeof(WriteIgnore))]
        public int TimeSignatureNum { get; set; } = 4;
        public int n { get => TimeSignatureNum; set => TimeSignatureNum = value; }
        
        [JsonProperty("timeSignatureDen")]
        [JsonConverter(typeof(WriteIgnore))]
        public int TimeSignatureDen { get; set; } = 4;
        public int d { get => TimeSignatureDen; set => TimeSignatureDen = value; }
        
        [JsonProperty("beatTuplets")]
        [JsonConverter(typeof(WriteIgnore))]
        public int[] BeatTuplets { get; set; } = Array.Empty<int>();
        public int[] bt { get => BeatTuplets; set => BeatTuplets = value; }
        
        public SongTimeChange() { }

        public SongTimeChange(float ts, float bpm, int tsn = 4, int tsd = 4, float? b = null, int[]? bt = null)
        {
            TimeStamp = ts;
            Bpm = bpm;
            TimeSignatureNum = tsn;
            TimeSignatureDen = tsd;
            BeatTime = b;
            BeatTuplets = bt ?? new int[] { 4, 4, 4, 4 };
        }
        
        public object Clone()
        {
            return new SongTimeChange(TimeStamp, Bpm, TimeSignatureNum, TimeSignatureDen, BeatTime, BeatTuplets);
        }
        
        SongTimeChange ICloneable<SongTimeChange>.Clone()
        {
            return (SongTimeChange)Clone();
        }

        public Latest.SongTimeChange Convert()
        {
            return new Latest.SongTimeChange(TimeStamp, Bpm, TimeSignatureNum, TimeSignatureDen, BeatTime, BeatTuplets);
        }

        public override string ToString()
        {
            return $"SongTimeChange({TimeStamp}ms,{Bpm}bpm)";
        }
    }

    public class SongPlayData : ICloneable<SongPlayData>, IVersionConvertible<Latest.SongPlayData>
    {
        [JsonProperty("songVariations")]
        public string[] SongVariations { get; set; } = Array.Empty<string>();
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();
        [JsonProperty("characters")]
        public SongCharacterData Characters { get; set; } = new SongCharacterData();
        [JsonProperty("stage")]
        public string Stage { get; set; } = string.Empty;
        [JsonProperty("noteSkin")]
        public string NoteSkin { get; set; } = string.Empty;
        [JsonProperty("stickerPack")]
        public string? StickerPack { get; set; }
        [JsonProperty("previewStart")]
        public int PreviewStart { get; set; } = 0;
        [JsonProperty("previewEnd")]
        public int PreviewEnd { get; set; } = 15000;
        
        public object Clone()
        {
            var res = new SongPlayData
            {
                SongVariations = (string[])SongVariations.Clone(),
                Difficulties = (string[])Difficulties.Clone(),
                Characters = (SongCharacterData)Characters.Clone(),
                Stage = Stage,
                NoteSkin = NoteSkin,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd
            };
            return res;
        }
        
        SongPlayData ICloneable<SongPlayData>.Clone()
        {
            return (SongPlayData)Clone();
        }

        public Latest.SongPlayData Convert()
        {
            return new Latest.SongPlayData()
            {
                SongVariations = (string[])SongVariations.Clone(),
                Difficulties = (string[])Difficulties.Clone(),
                Characters = Characters.Convert(),
                Stage = Stage,
                StickerPack = StickerPack,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd,
                Album = null,
                Ratings = new Dictionary<string, int>()
                {
                    { "default", 1 }
                },
                NoteStyle = NoteSkin
            };
        }

        public override string ToString()
        {
            return $"SongPlayData[LEGACY:v2.1.0]({JsonConvert.SerializeObject(SongVariations)}, {JsonConvert.SerializeObject(Difficulties)})";
        }
    }

    public class SongCharacterData : ICloneable<SongCharacterData>, IVersionConvertible<Latest.SongCharacterData>
    {
        [JsonProperty("player")]
        public string Player { get; set; } = string.Empty;
        [JsonProperty("girlfriend")]
        public string Girlfriend { get; set; } = string.Empty;
        [JsonProperty("opponent")]
        public string Opponent { get; set; } = string.Empty;
        [JsonProperty("instrumental")]
        public string Instrumental { get; set; } = string.Empty;
        [JsonProperty("altInstrumentals")]
        public string[] AltInstrumentals { get; set; } = Array.Empty<string>();
        [JsonProperty("opponentVocals")]
        public string[]? OpponentVocals { get; set; }
        [JsonProperty("playerVocals")]
        public string[]? PlayerVocals { get; set; }
        
        public SongCharacterData() {}

        public SongCharacterData(string player = "", string girlfriend = "", string opponent = "", string instrumental = "", string[]? altInstrumentals = null, string[]? opponentVocals = null, string[]? playerVocals = null)
        {
            Player = player;
            Girlfriend = girlfriend;
            Opponent = opponent;
            Instrumental = instrumental;
            AltInstrumentals = altInstrumentals ?? Array.Empty<string>();
            OpponentVocals = opponentVocals ?? new[] { opponent };
            PlayerVocals = playerVocals ?? new[] { player };
        }
        
        public object Clone()
        {
            var res = new SongCharacterData(Player, Girlfriend, Opponent, Instrumental)
            {
                AltInstrumentals = (string[])AltInstrumentals.Clone()
            };
            return res;
        }
        
        SongCharacterData ICloneable<SongCharacterData>.Clone()
        {
            return (SongCharacterData)Clone();
        }

        public Latest.SongCharacterData Convert()
        {
            return new Latest.SongCharacterData()
            {
                Player = Player,
                Girlfriend = Girlfriend,
                Opponent = Opponent,
                Instrumental = Instrumental,
                AltInstrumentals = AltInstrumentals,
                OpponentVocals = OpponentVocals,
                PlayerVocals = PlayerVocals
            };
        }

        public override string ToString()
        {
            return $"SongCharacterData({Player}, {Girlfriend}, {Opponent}, {Instrumental}, [${string.Join(", ", AltInstrumentals)}])";
        }
    }
}