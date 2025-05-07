using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest
{
    [Serializable]
    public class CharacterData : ICloneable<CharacterData>
    {
        /// <summary>
        /// Determines whether the specified property should be serialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be serialized.</returns>
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, CharacterData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "instrumental" => !string.IsNullOrEmpty(value as string),
                _ => true
            };
        }
        
        /// <summary>
        /// Determines whether the specified property should be deserialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be deserialized.</returns>
        public static bool ShouldDeserialize(JsonPropertyInfo propertyInfo, CharacterData instance, object? value)
        {
            return propertyInfo.Name switch
            {
                _ => true
            };
        }
        
        [JsonPropertyName("player")]
        public string Player { get; set; }

        [JsonPropertyName("girlfriend")]
        public string Girlfriend { get; set; }

        [JsonPropertyName("opponent")]
        public string Opponent { get; set; }
        
        [JsonPropertyName("instrumental")]
        [PredicateIgnore]
        public string Instrumental { get; set; }

        [JsonPropertyName("altInstrumentals")]
        public string[]? AltInstrumentals { get; set; } = null;
        
        [JsonPropertyName("opponentVocals")]
        private string[]? _OpponentVocals = null;

        [JsonIgnore]
        public string[]? OpponentVocals
        {
            get => _OpponentVocals ?? new[] { Opponent };
            set => _OpponentVocals = value;
        }

        [JsonPropertyName("playerVocals")]
        private string[]? _PlayerVocals { get; set; } = null;

        [JsonIgnore]
        public string[]? PlayerVocals
        {
            get => _PlayerVocals ?? new[] { Player };
            set => _PlayerVocals = value;
        }
        
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

        public CharacterData(string player = "", string girlfriend = "", string opponent = "", string instrumental = "", string[]? altInstrumentals = null, string[]? opponentVocals = null, string[]? playerVocals = null)
        {
            Player = player;
            Girlfriend = girlfriend;
            Opponent = opponent;
            Instrumental = instrumental;
            AltInstrumentals = altInstrumentals;
            OpponentVocals = opponentVocals;
            PlayerVocals = playerVocals;
        }
        
        public CharacterData CloneTyped()
        {
            return new CharacterData(Player, Girlfriend, Opponent, Instrumental)
            {
                AltInstrumentals = (string[]?)AltInstrumentals?.Clone(),
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }
        
        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        /// <returns>A string representation of the <see cref="CharacterData"/> instance.</returns>
        public override string ToString()
        {
            return $"SongCharacterData({Player}, {Girlfriend}, {Opponent}, {Instrumental}, [{string.Join(", ", AltInstrumentals ?? Array.Empty<string>())}])";
        }
    }
}