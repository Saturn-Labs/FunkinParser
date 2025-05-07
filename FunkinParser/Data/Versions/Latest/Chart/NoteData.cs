using System;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest.Chart
{
    public class NoteData : ICloneable<NoteData>
    {
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, NoteData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "time" or "data" or "length" or "kind" or "params" => false,
                _ => true
            };
        }
        
        [JsonPropertyName("time")]
        [PredicateIgnore]
        public float Time { get; set; } = 0.0f;
        public float t
        {
            get => Time;
            set => Time = value;
        }
        
        [JsonPropertyName("data")]
        [PredicateIgnore]
        public int Data { get; set; } = 0;
        public int d
        {
            get => Data;
            set => Data = value;
        }
        
        [JsonPropertyName("length")]
        [PredicateIgnore]
        public float Length { get; set; } = 0.0f;
        public float l
        {
            get => Length;
            set => Length = value;
        }
        
        [JsonPropertyName("kind")]
        [PredicateIgnore]
        public string? Kind { get; set; } = null;
        public string? k
        {
            get => Kind;
            set => Kind = value;
        }
        
        [JsonPropertyName("params")]
        [PredicateIgnore]
        public NoteParamData[] Parameters { get; set; } = Array.Empty<NoteParamData>();
        public NoteParamData[] p
        {
            get => Parameters;
            set => Parameters = value;
        }

        public NoteData() {}
        public NoteData(float time, int data, float length = .0f, string? kind = null, NoteParamData[]? parameters = null) : this()
        {
            Time = time;
            Data = data;
            Length = length;
            Kind = kind;
            Parameters = parameters ?? Array.Empty<NoteParamData>();
        }
        
        public NoteData CloneTyped()
        {
            return new NoteData(Time, Data, Length, Kind, (NoteParamData[])Parameters.Clone());
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public override string ToString()
        {
            return $"SongNoteData({Time}ms, {Data}, {Length}ms hold, {(string.IsNullOrEmpty(Kind) ? "default" : Kind)}, {Parameters.Length} params)";
        }
    }
}