using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest.Chart
{
    public class NoteParamData : ICloneable<NoteParamData>
    {
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, NoteParamData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "name" or "value" => false,
                _ => true
            };
        }
        
        [JsonPropertyName("name")]
        [PredicateIgnore]
        public string Name { get; set; }
        public string n
        {
            get => Name;
            set => Name = value;
        }
        
        [JsonPropertyName("value")]
        [PredicateIgnore]
        public object? Value { get; set; } = null;
        public object? v
        {
            get => Value;
            set => Value = value;
        }
        
        public NoteParamData(string name, object? value)
        {
            Name = name;
            Value = value;
        }
        
        public NoteParamData CloneTyped()
        {
            return new NoteParamData(Name, Value);
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public override string ToString()
        {
            return $"NoteParamData({Name}, {Value})";
        }
    }
}