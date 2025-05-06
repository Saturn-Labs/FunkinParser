using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funkin.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NuGet.Versioning;

namespace Funkin.Core.Data.Latest
{
    public class SongChartData : VersioningData, IData, ICloneable<SongChartData>
    {
        public static readonly NuGetVersion DefaultVersion = new("2.0.0");
        
        [JsonProperty("scrollSpeed")]
        public Dictionary<string, float> ScrollSpeed { get; set; } = new();
        [JsonProperty("events")]
        public SongEventData[] Events { get; set; } = Array.Empty<SongEventData>();
        [JsonProperty("notes")] 
        public Dictionary<string, SongNoteData[]> Notes { get; set; } = new();
        [JsonProperty("generatedBy")] 
        public string GeneratedBy { get; set; } = "Unknown";
        [JsonIgnore] 
        public string Variation { get; set; } = "default";

        public SongChartData()
        {
            Version = DefaultVersion;
        }
        
        public SongChartData(Dictionary<string, float> scrollSpeed, SongEventData[] events, Dictionary<string, SongNoteData[]> notes) : this()
        {
            ScrollSpeed = scrollSpeed;
            Events = events;
            Notes = notes;
        }
        
        private static readonly PropertyInfo[] PropertiesCache = typeof(SongChartData).GetProperties();
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

        public SongChartData Clone()
        {
            var rec = new SongChartData(ScrollSpeed, Events.Select(s => s.Clone()).ToArray(), new Dictionary<string, SongNoteData[]>())
            {
                Notes = Notes.Select(kv =>
                {
                    var array = new SongNoteData[kv.Value.Length];
                    for (var i = 0; i < kv.Value.Length; i++)
                    {
                        array[i] = kv.Value[i].Clone();
                    }
                    return new KeyValuePair<string, SongNoteData[]>(kv.Key, array);
                }).ToDictionary(o => o.Key, o => o.Value),
                Version = Version,
                GeneratedBy = GeneratedBy,
                Variation = Variation
            };
            return rec;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public class SongEventData : ICloneable<SongEventData>
    {
        [JsonProperty("time")] 
        [JsonConverter(typeof(WriteIgnore))]
        public float Time { get; set; } = 0.0f;
        [JsonProperty("t")]
        public float t 
        { 
            get => Time;
            set => Time = value;
        }
        [JsonProperty("eventKind")]
        [JsonConverter(typeof(WriteIgnore))]
        public string EventKind { get; set; } = string.Empty;
        [JsonProperty("value")]
        [JsonConverter(typeof(WriteIgnore))]
        public object? Value { get; set; } = null;

        [JsonProperty("v")]
        public object? v
        {
            get => Value; 
            set => Value = value; 
        }
        
        public SongEventData() { }
        public SongEventData(float time, string eventKind, object? value)
        {
            Time = time;
            EventKind = eventKind;
            Value = value;
        }
        
        public SongEventData Clone()
        {
            return new SongEventData(Time, EventKind, Value);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public class SongNoteData : ICloneable<SongNoteData>
    {
        [JsonProperty("time")]
        [JsonConverter(typeof(WriteIgnore))]
        public float Time { get; set; } = 0.0f;
        [JsonProperty("t")]
        public float t
        {
            get => Time; 
            set => Time = value;
        }
        [JsonProperty("data")]
        [JsonConverter(typeof(WriteIgnore))]
        public int Data { get; set; } = 0;
        [JsonProperty("d")]
        public int d
        {
            get => Data;
            set => Data = value;
        }
        [JsonProperty("length")]
        [JsonConverter(typeof(WriteIgnore))]
        public float Length { get; set; } = 0.0f;
        [JsonProperty("l")]
        public float l
        {
            get => Length;
            set => Length = value;
        }
        [JsonProperty("kind")]
        [JsonConverter(typeof(WriteIgnore))]
        public string? Kind { get; set; } = null;
        [JsonProperty("k")]
        public string? k
        {
            get => Kind;
            set => Kind = value;
        }
        [JsonProperty("params")]
        [JsonConverter(typeof(WriteIgnore))]
        public NoteParamData[] Params { get; set; } = Array.Empty<NoteParamData>();
        [JsonProperty("p")]
        public NoteParamData[] p
        {
            get => Params;
            set => Params = value;
        }
        
        public SongNoteData Clone()
        {
            return new SongNoteData()
            {
                Time = Time,
                Data = Data,
                Length = Length,
                Kind = Kind,
                Params = Params.Select(o => o.Clone()).ToArray()
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public class NoteParamData : ICloneable<NoteParamData>
    {
        [JsonProperty("name")]
        [JsonConverter(typeof(WriteIgnore))]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("n")]
        public string n
        {
            get => Name;
            set => Name = value;
        }
        [JsonProperty("value")]
        [JsonConverter(typeof(WriteIgnore))]
        public object? Value { get; set; } = null;
        [JsonProperty("v")]
        public object? v
        {
            get => Value;
            set => Value = value;
        }
        
        public NoteParamData() { }
        public NoteParamData(string name, object? value)
        {
            Name = name;
            Value = value;
        }
        
        public NoteParamData Clone()
        {
            return new NoteParamData(Name, Value);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}