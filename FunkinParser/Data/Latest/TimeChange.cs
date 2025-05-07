using System.Collections.Generic;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funkin.Data.Latest
{
    public class TimeChange : ICloneable<TimeChange>
    {
        public static readonly int[] DefaultBeatTuplets = { 4, 4, 4, 4 };

        /**
         * Timestamp in specified `timeFormat`.
         */
        [JsonConverter(typeof(AliasConverter), "timeStamp", "t")]
        public float TimeStamp { get; set; }
        
        /**
         * Time in beats (int). The game will calculate further beat values based on this one,
         * so it can do it in a simple linear fashion.
         */
        [JsonConverter(typeof(AliasConverter), "beatTime", "b")]
        public float? BeatTime { get; set; }
        
        /**
         * Quarter notes per minute (float). Cannot be empty in the first element of the list,
         * but otherwise it's optional, and defaults to the value of the previous element.
         */
        [JsonProperty("bpm")]
        public float BeatsPerMinute { get; set; }
        
        /**
         * Time signature numerator (int). Optional, defaults to 4.
         */
        [JsonConverter(typeof(AliasConverter), "timeSignatureNum", "n")]
        public int TimeSignatureNumerator { get; set; } = 4;
        
        /**
         * Time signature denominator (int). Optional, defaults to 4. Should only ever be a power of two.
         */
        [JsonConverter(typeof(AliasConverter), "timeSignatureDen", "d")]
        public int TimeSignatureDenominator { get; set; } = 4;
        
        /**
         * Beat tuplets (int[] or int). This defines how many steps each beat is divided into.
         * It can either be an array of length `n` (see above) or a single integer number.
         * Optional, defaults to `[4]`.
         */
        [JsonConverter(typeof(AliasConverter), "beatTuplets", "bt")]
        public int[] BeatTuplets { get; set; } = DefaultBeatTuplets;
        
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }
        
        public TimeChange() {}
        public TimeChange(float timeStamp, float bpm, int timeSignatureNum = 4, int timeSignatureDen = 4, float? beatTime = null, int[]? beatTuplets = null) : this()
        {
            TimeStamp = timeStamp;
            BeatsPerMinute = bpm;
            TimeSignatureNumerator = timeSignatureNum;
            TimeSignatureDenominator = timeSignatureDen;
            BeatTime = beatTime;
            BeatTuplets = beatTuplets ?? DefaultBeatTuplets;
        }
        
        public TimeChange CloneTyped()
        {
            return new TimeChange(TimeStamp, BeatsPerMinute, TimeSignatureNumerator, TimeSignatureDenominator, BeatTime, (int[])BeatTuplets.Clone())
            {
                ExtensionData = new Dictionary<string, JToken>(ExtensionData ?? new Dictionary<string, JToken>()),
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
            return $"SongTimeChange({TimeStamp}ms, {BeatsPerMinute}bpm)";
        }
    }
}