using System;
using System.Collections.Generic;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funkin.Data.Latest
{
    public class CharacterData : ICloneable<CharacterData>
    {
        [JsonProperty("player")]
        public string Player { get; set; }

        [JsonProperty("girlfriend")]
        public string Girlfriend { get; set; }

        [JsonProperty("opponent")]
        public string Opponent { get; set; }
        
        [JsonProperty("instrumental")]
        public string Instrumental { get; set; }
        
        [JsonProperty("altInstrumentals")]
        public string[] AltInstrumentals { get; set; }
        
        [JsonProperty("opponentVocals")]
        public string[]? OpponentVocals { get; set; } = null;
        
        [JsonProperty("playerVocals")]
        public string[]? PlayerVocals { get; set; } = null;
        
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }

        public CharacterData(string player = "", string girlfriend = "", string opponent = "", string instrumental = "", string[]? altInstrumentals = null, string[]? opponentVocals = null, string[]? playerVocals = null)
        {
            Player = player;
            Girlfriend = girlfriend;
            Opponent = opponent;
            Instrumental = instrumental;
            AltInstrumentals = altInstrumentals ?? Array.Empty<string>();
            OpponentVocals = opponentVocals ?? new[] { opponent };
            PlayerVocals = playerVocals ?? new[] { player };
        }
        
        public CharacterData CloneTyped()
        {
            return new CharacterData(Player, Girlfriend, Opponent, Instrumental)
            {
                AltInstrumentals = (string[])AltInstrumentals.Clone()
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        /**
         * Produces a string representation suitable for debugging.
         */
        public override string ToString()
        {
            return $"SongCharacterData({Player}, {Girlfriend}, {Opponent}, {Instrumental}, [{string.Join(", ", AltInstrumentals)}])";
        }
    }
}