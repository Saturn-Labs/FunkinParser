using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Funkin.Core.Data.v20X
{
    public class SongData : VersioningData, IData, ICloneable<SongData>, IVersionConvertible<v21X.SongData>
    {
        public static readonly NuGetVersion DefaultVersion = new("2.0.0");
        
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
                    if (p.GetCustomAttribute<JsonPropertyAttribute>() is not { } attr)
                        return false;
                    return attr.PropertyName == key;
                }
            ) is not null;
        }

        public v21X.SongData Convert()
        {
            return new v21X.SongData(SongName, Artist, Variation)
            {
                TimeFormat = TimeFormat,
                Divisions = Divisions,
                TimeChanges = TimeChanges.Select(t => t.Convert()).ToArray(),
                Looped = Looped,
                PlayData = PlayData?.Convert() ?? new v21X.SongPlayData(),
                GeneratedBy = GeneratedBy
            };
        }

        public override string ToString()
        {
            return $"SongMetadata[LEGACY:v2.2.0]({SongName} by {Artist}, variation {Variation})";
        }
    }

    public class SongTimeChange : ICloneable<SongTimeChange>, IVersionConvertible<v21X.SongTimeChange>
    {
        [JsonProperty("timeStamp")]
        [JsonConverter(typeof(WriteIgnore))]
        public float TimeStamp { get; set; } = 0f;
        public float t
        {
            get => TimeStamp; 
            set => TimeStamp = value;
        }
        [JsonProperty("beatTime")]
        [JsonConverter(typeof(WriteIgnore))]
        public float? BeatTime { get; set; } = 0f;
        public float? b
        {
            get => BeatTime; 
            set => BeatTime = value;
        }
        [JsonProperty("bpm")] 
        public float Bpm { get; set; } = 0f;
        [JsonProperty("timeSignatureNum")]
        [JsonConverter(typeof(WriteIgnore))]
        public int TimeSignatureNum { get; set; } = 4;
        public int n
        {
            get => TimeSignatureNum;
            set => TimeSignatureNum = value;
        }
        [JsonProperty("timeSignatureDen")]
        [JsonConverter(typeof(WriteIgnore))]
        public int TimeSignatureDen { get; set; } = 4;
        public int d
        {
            get => TimeSignatureDen; 
            set => TimeSignatureDen = value;
        }
        [JsonProperty("beatTuplets")]
        [JsonConverter(typeof(WriteIgnore))]
        public int[] BeatTuplets { get; set; } = Array.Empty<int>();
        public int[] bt
        {
            get => BeatTuplets; 
            set => BeatTuplets = value;
        }
        
        public SongTimeChange() { }
        public SongTimeChange(float ts, float bpm, int tsn = 4, int tsd = 4, float? b = null, int[]? bt = null)
        {
            TimeStamp = ts;
            Bpm = bpm;
            TimeSignatureNum = tsn;
            TimeSignatureDen = tsd;
            BeatTime = b;
            BeatTuplets = bt ?? new[] { 4, 4, 4, 4 };
        }
        
        public object Clone()
        {
            return new SongTimeChange(TimeStamp, Bpm, TimeSignatureNum, TimeSignatureDen, BeatTime, BeatTuplets);
        }
        
        SongTimeChange ICloneable<SongTimeChange>.Clone()
        {
            return (SongTimeChange)Clone();
        }

        public v21X.SongTimeChange Convert()
        {
            return new v21X.SongTimeChange(TimeStamp, Bpm, TimeSignatureNum, TimeSignatureDen, BeatTime, BeatTuplets);
        }

        public override string ToString()
        {
            return $"SongTimeChange({TimeStamp}ms,{Bpm}bpm)";
        }
    }

    public class SongPlayData : ICloneable<SongPlayData>, IVersionConvertible<v21X.SongPlayData>
    {
        [JsonProperty("songVariations")]
        public string[] SongVariations { get; set; } = Array.Empty<string>();
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; } = Array.Empty<string>();
        [JsonProperty("playableChars")]
        public Dictionary<string, SongPlayableChar> PlayableChars { get; set; } = new();   
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
                PlayableChars = PlayableChars.Select(kv => new KeyValuePair<string, SongPlayableChar>(kv.Key, kv.Value.Clone())).ToDictionary(kv => kv.Key, kv => kv.Value),
                Stage = Stage,
                NoteSkin = NoteSkin,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd,
                StickerPack = StickerPack
            };
            return res;
        }
        
        SongPlayData ICloneable<SongPlayData>.Clone()
        {
            return (SongPlayData)Clone();
        }

        public v21X.SongPlayData Convert()
        {
            var characterData = new v21X.SongCharacterData("bf", "gf", "dad");
            PlayableChars.FirstOrDefault().Deconstruct(out var firstCharKey, out var firstCharData);
            if (firstCharKey != null && firstCharData != null)
            {
                characterData = new v21X.SongCharacterData(firstCharKey, firstCharData.Girlfriend, firstCharData.Opponent, firstCharData.Inst);
            }
            return new v21X.SongPlayData()
            {
                SongVariations = (string[])SongVariations.Clone(),
                Difficulties = (string[])Difficulties.Clone(),
                Stage = Stage,
                NoteSkin = NoteSkin,
                PreviewStart = PreviewStart,
                PreviewEnd = PreviewEnd,
                StickerPack = StickerPack,
                Characters = characterData,
            };
        }

        public override string ToString()
        {
            return $"SongPlayData[LEGACY:v2.0.0]({JsonConvert.SerializeObject(SongVariations)}, {JsonConvert.SerializeObject(Difficulties)})";
        }
    }

    public class SongPlayableChar : ICloneable<SongPlayableChar>
    {
        [JsonProperty("girlfriend")]
        [JsonConverter(typeof(WriteIgnore))]
        public string Girlfriend { get; set; } = string.Empty;
        [JsonProperty("g")]
        public string g
        {
            get => Girlfriend;
            set => Girlfriend = value;
        }
        [JsonProperty("opponent")]
        [JsonConverter(typeof(WriteIgnore))]
        public string Opponent { get; set; } = string.Empty;
        [JsonProperty("o")]
        public string o
        {
            get => Opponent;
            set => Opponent = value;
        }
        [JsonProperty("inst")]
        [JsonConverter(typeof(WriteIgnore))]
        public string Inst { get; set; } = string.Empty;
        [JsonProperty("i")]
        public string i
        {
            get => Inst;
            set => Inst = value;
        }
        
        public SongPlayableChar Clone()
        {
            return new SongPlayableChar
            {
                Girlfriend = Girlfriend,
                Opponent = Opponent,
                Inst = Inst
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
            return $"SongPlayableChar[LEGACY:v2.0.0](${Girlfriend}, ${Opponent}, ${Inst})";
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
}