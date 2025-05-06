using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Core.Data.Latest
{
    public class SongData : VersioningData, IData, ICloneable<SongData>
    {
        public static readonly NuGetVersion DefaultVersion = new("2.2.4");
        
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
        [JsonProperty("offsets")]
        public SongOffsets? Offsets { get; set; } = null;
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
                Offsets = Offsets?.Clone() as SongOffsets ?? new SongOffsets(),
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
                if (p.GetCustomAttribute<JsonPropertyAttribute>() is not {} attr)
                    return false;
                return attr.PropertyName == key;
            });
            return prop?.GetValue(this);
        }

        public T GetValue<T>(string key)
        {
            if (GetValue(key) is not T value)
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
            ) is not null;
        }
        
        public override string ToString()
        {
            return $"SongMetadata({SongName} by {Artist}, variation {Variation})";
        }
    }

    public class SongTimeChange : ICloneable<SongTimeChange>
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
        
        public override string ToString()
        {
            return $"SongTimeChange({TimeStamp}ms,{Bpm}bpm)";
        }
    }

    public class SongPlayData : ICloneable<SongPlayData>
    {
        [JsonProperty("songVariations")]
        public string[] SongVariations { get; set; } = Array.Empty<string>();
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();
        [JsonProperty("characters")]
        public SongCharacterData Characters { get; set; } = new SongCharacterData();
        [JsonProperty("stage")]
        public string Stage { get; set; } = string.Empty;
        [JsonProperty("noteStyle")]
        public string NoteStyle { get; set; } = string.Empty;
        [JsonProperty("ratings")]
        public Dictionary<string, int> Ratings { get; set; } = new Dictionary<string, int>()
        {
            { "normal", 0 }
        };
        [JsonProperty("album")]
        public string? Album { get; set; }
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
                NoteStyle = NoteStyle,
                Ratings = new Dictionary<string, int>(Ratings),
                Album = Album,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd
            };
            return res;
        }
        
        SongPlayData ICloneable<SongPlayData>.Clone()
        {
            return (SongPlayData)Clone();
        }

        public override string ToString()
        {
            return $"SongPlayData({JsonConvert.SerializeObject(SongVariations)}, {JsonConvert.SerializeObject(Difficulties)})";
        }
    }

    public class SongCharacterData : ICloneable<SongCharacterData>
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

        public override string ToString()
        {
            return $"SongCharacterData({Player}, {Girlfriend}, {Opponent}, {Instrumental}, [${string.Join(", ", AltInstrumentals)}])";
        }
    }

    public class SongOffsets : ICloneable<SongOffsets>
    {
        [JsonProperty("instrumental")]
        public float Instrumental = 0.0f;
        [JsonProperty("altInstrumentals")]
        public Dictionary<string, float> AltInstrumentals = new Dictionary<string, float>();
        [JsonProperty("vocals")]
        public Dictionary<string, float> Vocals = new Dictionary<string, float>();
        [JsonProperty("altVocals")]
        Dictionary<string, Dictionary<string, float>> AltVocals = new Dictionary<string, Dictionary<string, float>>();
    
        public SongOffsets() { }
        public SongOffsets(float inst = 0.0f, Dictionary<string, float>? altInsts = null, Dictionary<string, float>? vocals = null, Dictionary<string, Dictionary<string, float>>? altVocals = null)
        {
        
        }
    
        public object Clone()
        {
            var res = new SongOffsets(Instrumental)
            {
                AltInstrumentals = new Dictionary<string, float>(AltInstrumentals),
                Vocals = new Dictionary<string, float>(Vocals)
            };
            return res;
        }

        SongOffsets ICloneable<SongOffsets>.Clone()
        {
            return (SongOffsets)Clone();
        }

        public override string ToString()
        {
            return $"SongOffsets({Instrumental}ms, {JsonConvert.SerializeObject(AltInstrumentals)}, {JsonConvert.SerializeObject(Vocals)}, {JsonConvert.SerializeObject(AltVocals)})";
        }
    }
}