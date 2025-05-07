using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Data.Versions.Latest;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.v200
{
    public class PlayableChar : ICloneable<PlayableChar>, IConvertible<Latest.CharacterData>
    {
        /// <summary>
        /// Determines whether the specified property should be serialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be serialized.</returns>
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, PlayableChar instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "i" => !string.IsNullOrEmpty(value as string),
                "girlfriend" or "opponent" or "inst" => false,
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
        public static bool ShouldDeserialize(JsonPropertyInfo propertyInfo, PlayableChar instance, object? value)
        {
            return propertyInfo.Name switch
            {
                _ => true
            };
        }

        /// <summary>
        /// The girlfriend's character ID.
        /// </summary>
        [JsonPropertyName("girlfriend")]
        [PredicateIgnore]
        public string Girlfriend { get; set; }

        /// <summary>
        /// Alias for <see cref="Girlfriend"/>.
        /// </summary>
        public string g
        {
            get => Girlfriend;
            set => Girlfriend = value;
        }
        
        /// <summary>
        /// The opponent's character ID.
        /// </summary>
        [JsonPropertyName("opponent")]
        [PredicateIgnore]
        public string Opponent { get; set; }

        /// <summary>
        /// Alias for <see cref="Opponent"/>.
        /// </summary>
        public string o
        {
            get => Opponent;
            set => Opponent = value;
        }
        
        /// <summary>
        /// The instrumental.
        /// </summary>
        [JsonPropertyName("inst")]
        [PredicateIgnore]
        public string Instrumental { get; set; }

        /// <summary>
        /// Alias for <see cref="Instrumental"/>.
        /// </summary>
        public string i
        {
            get => Instrumental;
            set => Instrumental = value;
        }
        
        public PlayableChar(string girlfriend = "gf", string opponent = "dad", string instrumental = "")
        {
            Girlfriend = girlfriend;
            Opponent = opponent;
            Instrumental = instrumental;
        }
        
        public PlayableChar CloneTyped()
        {
            return new PlayableChar(Girlfriend, Opponent, Instrumental);
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public bool TryConvert(out CharacterData? result)
        {
            result = new CharacterData("bf", Girlfriend, Opponent, Instrumental);
            return true;
        }

        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        /// <returns>A string representation of the PlayableChar.</returns>
        public override string ToString()
        {
            return $"SongPlayableChar[LEGACY:v2.0.0]({Girlfriend}, {Opponent}, {Instrumental})";
        }
    }
}