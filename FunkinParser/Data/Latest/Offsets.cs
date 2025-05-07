using System;
using System.Collections.Generic;
using System.Linq;
using Funkin.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funkin.Data.Latest
{
    /**
     * Offsets to apply to the song's instrumental and vocals, relative to the chart.
     * These are intended to correct for issues with the chart, or with the song's audio (for example a 10ms delay before the song starts).
     * This is independent of the offsets applied in the user's settings, which are applied after these offsets and intended to correct for the user's hardware.
     */
    [Serializable]
    public class Offsets : ICloneable<Offsets>
    {
        /**
         * The offset, in milliseconds, to apply to the song's instrumental relative to the chart.
         * For example, setting this to `-10.0` will start the instrumental 10ms earlier than the chart.
         *
         * Setting this to `-5000.0` means the chart start 5 seconds into the song.
         * Setting this to `5000.0` means there will be 5 seconds of silence before the song starts.
         */
        [JsonProperty("instrumental")]
        public float Instrumental { get; set; }
        
        /**
         * Apply different offsets to different alternate instrumentals.
         */
        [JsonProperty("altInstrumentals")]
        public Dictionary<string, float> AltInstrumentals { get; set; }
        
        /**
         * The offset, in milliseconds, to apply to the song's vocals, relative to the song's base instrumental.
         * These are applied ON TOP OF the instrumental offset.
         */
        [JsonProperty("vocals")]
        public Dictionary<string, float> Vocals { get; set; }
        
        /**
         * The offset, in milliseconds, to apply to the songs vocals, relative to each alternate instrumental.
         * This is useful for the circumstance where, for example, an alt instrumental has a few seconds of lead in before the song starts.
         */
        [JsonProperty("altVocals")]
        public Dictionary<string, Dictionary<string, float>> AltVocals { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }

        public Offsets(float instrumental = 0, Dictionary<string, float>? altInstrumentals = null, Dictionary<string, float>? vocals = null, Dictionary<string, Dictionary<string, float>>? altVocals = null)
        {
            Instrumental = instrumental;
            AltInstrumentals = altInstrumentals ?? new();
            Vocals = vocals ?? new();
            AltVocals = altVocals ?? new();
        }

        public float GetInstrumentalOffset(string? instrumental = null)
        {
            return string.IsNullOrEmpty(instrumental) ? Instrumental : AltInstrumentals.GetValueOrDefault(instrumental, Instrumental);
        }

        public float SetInstrumentalOffset(float offset, string? instrumental = null)
        {
            if (string.IsNullOrEmpty(instrumental))
            {
                Instrumental = offset;
                return offset;
            }
            AltInstrumentals[instrumental] = offset;
            return offset;
        }

        public float GetVocalOffset(string charId, string? instrumental = null)
        {
            if (string.IsNullOrEmpty(instrumental))
            {
                return Vocals.GetValueOrDefault(charId, 0.0f);
            }

            return !AltVocals.TryGetValue(charId, out var dict) ? 0.0f : dict.GetValueOrDefault(instrumental, 0.0f);
        }

        public float SetVocalOffset(string charId, float offset)
        {
            Vocals[charId] = offset;
            return offset;
        }
        
        public Offsets CloneTyped()
        {
            return new Offsets(Instrumental)
            {
                AltInstrumentals = new Dictionary<string, float>(AltInstrumentals),
                Vocals = new Dictionary<string, float>(Vocals),
                AltVocals = AltVocals.ToDictionary(
                    outer => outer.Key,
                    outer => new Dictionary<string, float>(outer.Value)
                ),
                ExtensionData = new Dictionary<string, JToken>(ExtensionData ?? new Dictionary<string, JToken>())
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
            return $"SongOffsets({Instrumental}ms, {JsonConvert.SerializeObject(AltInstrumentals)}, {JsonConvert.SerializeObject(Vocals)}, {JsonConvert.SerializeObject(AltVocals)})";
        }
    }
}