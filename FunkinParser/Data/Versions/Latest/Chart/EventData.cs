using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest.Chart
{
    public class EventData : ICloneable<EventData>
    {
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, EventData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "time" or "eventKind" or "value" => false,
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
        
        [JsonPropertyName("eventKind")]
        [PredicateIgnore]
        public string EventKind { get; set; }
        public string e
        {
            get => EventKind;
            set => EventKind = value;
        }
        
        [JsonPropertyName("value")]
        [PredicateIgnore]
        public object? Value { get; set; }
        public object? v
        {
            get => Value;
            set => Value = value;
        }
        
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

        public EventData(float time, string eventKind, object? value = null)
        {
            Time = time;
            EventKind = eventKind;
            Value = value;
        }
        
        public EventData CloneTyped()
        {
            return new EventData(Time, EventKind, Value is ICloneable cloneable ? cloneable.Clone() : Value);
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public override string ToString()
        {
            return $"SongEventData({Time}ms, {EventKind}: {Value})";
        }
    }
}