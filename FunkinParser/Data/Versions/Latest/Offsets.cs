using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest
{
    /// <summary>
    /// Offsets to apply to the song's instrumental and vocals, relative to the chart.
    /// These are intended to correct for issues with the chart, or with the song's audio 
    /// (for example a 10ms delay before the song starts).
    /// This is independent of the offsets applied in the user's settings, which are applied 
    /// after these offsets and intended to correct for the user's hardware.
    /// </summary>
    [Serializable]
    public class Offsets : ICloneable<Offsets>
    {
        /// <summary>
        /// The offset, in milliseconds, to apply to the song's instrumental relative to the chart.
        /// For example, setting this to <c>-10.0</c> will start the instrumental 10ms earlier than the chart.
        /// Setting this to <c>-5000.0</c> means the chart starts 5 seconds into the song.
        /// Setting this to <c>5000.0</c> means there will be 5 seconds of silence before the song starts.
        /// </summary>
        [JsonPropertyName("instrumental")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float Instrumental { get; set; }

        /// <summary>
        /// Apply different offsets to different alternate instrumentals.
        /// </summary>
        [JsonPropertyName("altInstrumentals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Dictionary<string, float>? AltInstrumentals { get; set; }

        /// <summary>
        /// The offset, in milliseconds, to apply to the song's vocals, relative to the song's base instrumental.
        /// These are applied ON TOP OF the instrumental offset.
        /// </summary>
        [JsonPropertyName("vocals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Dictionary<string, float>? Vocals { get; set; }

        /// <summary>
        /// The offset, in milliseconds, to apply to the song's vocals, relative to each alternate instrumental.
        /// This is useful for the circumstance where, for example, an alt instrumental has a few seconds of lead-in before the song starts.
        /// </summary>
        [JsonPropertyName("altVocals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Dictionary<string, Dictionary<string, float>>? AltVocals { get; set; }

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Offsets"/> class with default or specified values.
        /// </summary>
        /// <param name="instrumental">The offset for the instrumental in milliseconds.</param>
        /// <param name="altInstrumentals">Offsets for alternate instrumentals.</param>
        /// <param name="vocals">Offsets for vocals.</param>
        /// <param name="altVocals">Offsets for vocals relative to alternate instrumentals.</param>
        public Offsets(float instrumental = 0, Dictionary<string, float>? altInstrumentals = null, Dictionary<string, float>? vocals = null, Dictionary<string, Dictionary<string, float>>? altVocals = null)
        {
            Instrumental = instrumental;
            AltInstrumentals = altInstrumentals;
            Vocals = vocals;
            AltVocals = altVocals;
        }

        /// <summary>
        /// Gets the offset for the specified instrumental or the default instrumental offset if none is specified.
        /// </summary>
        /// <param name="instrumental">The name of the instrumental (optional).</param>
        /// <returns>The offset in milliseconds.</returns>
        public float GetInstrumentalOffset(string? instrumental = null)
        {
            return string.IsNullOrEmpty(instrumental) ? Instrumental : AltInstrumentals.GetValueOrDefault(instrumental, Instrumental);
        }

        /// <summary>
        /// Sets the offset for the specified instrumental or updates the default instrumental offset.
        /// </summary>
        /// <param name="offset">The offset in milliseconds.</param>
        /// <param name="instrumental">The name of the instrumental (optional).</param>
        /// <returns>The updated offset in milliseconds.</returns>
        public float SetInstrumentalOffset(float offset, string? instrumental = null)
        {
            if (string.IsNullOrEmpty(instrumental) || AltInstrumentals is null)
            {
                Instrumental = offset;
                return offset;
            }
            AltInstrumentals[instrumental] = offset;
            return offset;
        }

        /// <summary>
        /// Gets the vocal offset for the specified character and instrumental.
        /// </summary>
        /// <param name="charId">The character ID.</param>
        /// <param name="instrumental">The name of the instrumental (optional).</param>
        /// <returns>The vocal offset in milliseconds.</returns>
        public float GetVocalOffset(string charId, string? instrumental = null)
        {
            if (string.IsNullOrEmpty(instrumental) || AltVocals is null)
            {
                return Vocals?.GetValueOrDefault(charId, 0.0f) ?? 0.0f;
            }

            return !AltVocals.TryGetValue(charId, out var dict) ? 0.0f : dict.GetValueOrDefault(instrumental, 0.0f);
        }

        /// <summary>
        /// Sets the vocal offset for the specified character.
        /// </summary>
        /// <param name="charId">The character ID.</param>
        /// <param name="offset">The offset in milliseconds.</param>
        /// <returns>The updated offset in milliseconds.</returns>
        public float SetVocalOffset(string charId, float offset)
        {
            if (Vocals is null)
                return offset;
            Vocals[charId] = offset;
            return offset;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="Offsets"/> instance.
        /// </summary>
        /// <returns>A new <see cref="Offsets"/> instance with the same data.</returns>
        public Offsets CloneTyped()
        {
            return new Offsets(Instrumental)
            {
                AltInstrumentals = AltInstrumentals is null ? null : new Dictionary<string, float>(AltInstrumentals),
                Vocals = Vocals is null ? null : new Dictionary<string, float>(Vocals),
                AltVocals = AltVocals?.ToDictionary(
                    outer => outer.Key,
                    outer => new Dictionary<string, float>(outer.Value)
                ),
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>())
            };
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="Offsets"/> instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return CloneTyped();
        }

        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        /// <returns>A string representation of the <see cref="Offsets"/> instance.</returns>
        public override string ToString()
        {
            return $"SongOffsets({Instrumental}ms, {JsonSerializer.Serialize(AltInstrumentals)}, {JsonSerializer.Serialize(Vocals)}, {JsonSerializer.Serialize(AltVocals)})";
        }

        public bool IsDefault()
        {
            return this == new Offsets();
        }
        
        public static bool operator ==(Offsets left, Offsets right)
        {
            return Equals(left, right);
        }
        
        public static bool operator !=(Offsets left, Offsets right)
        {
            return !Equals(left, right);
        }
        
        public override bool Equals(object? obj)
        {
            return Equals(obj as Offsets);
        }

        protected bool Equals(Offsets? other)
        {
            if (other is null) 
                return false;
            return Instrumental.Equals(other.Instrumental) && Equals(AltInstrumentals, other.AltInstrumentals) && Equals(Vocals, other.Vocals) && Equals(AltVocals, other.AltVocals) && Equals(ExtensionData, other.ExtensionData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Instrumental, AltInstrumentals, Vocals, AltVocals, ExtensionData);
        }
    }
}