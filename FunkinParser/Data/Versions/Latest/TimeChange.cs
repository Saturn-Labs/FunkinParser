﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Funkin.Data.Attributes;
using Funkin.Data.Converters;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.Latest
{
    /// <summary>
    /// Represents a time change in a song, including timestamp, BPM, and time signature details.
    /// </summary>
    [Serializable]
    public class TimeChange : ICloneable<TimeChange>
    {
        /// <summary>
        /// Determines whether the specified property should be serialized (Called via reflection for each member with <see cref="PredicateIgnoreAttribute"/>).
        /// </summary>
        /// <param name="propertyInfo"><see cref="JsonPropertyInfo"/> of that specified property.</param>
        /// <param name="instance">Instance of the current class.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>If that property should be serialized.</returns>
        public static bool ShouldSerialize(JsonPropertyInfo propertyInfo, TimeChange instance, object? value)
        {
            return propertyInfo.Name switch
            {
                "timeStamp" or "beatTime" or "timeSignatureNum" or "timeSignatureDen" or "beatTuplets" => false,
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
        public static bool ShouldDeserialize(JsonPropertyInfo propertyInfo, TimeChange instance, object? value)
        {
            return propertyInfo.Name switch
            {
                _ => true
            };
        }
        
        /// <summary>
        /// Default beat tuplets used when none are specified.
        /// </summary>
        public static readonly int[] DefaultBeatTuplets = { 4, 4, 4, 4 };

        /// <summary>
        /// Timestamp in specified `timeFormat`.
        /// </summary>
        [JsonPropertyName("timeStamp")]
        [PredicateIgnore]
        public float TimeStamp { get; set; }

        /// <summary>
        /// Alias for <see cref="TimeStamp"/>.
        /// </summary>
        public float t
        {
            get => TimeStamp;
            set => TimeStamp = value;
        }

        /// <summary>
        /// Time in beats (int). The game will calculate further beat values based on this one,
        /// so it can do it in a simple linear fashion.
        /// </summary>
        [JsonPropertyName("beatTime")]
        [PredicateIgnore]
        public float? BeatTime { get; set; }

        /// <summary>
        /// Alias for <see cref="BeatTime"/>.
        /// </summary>
        public float? b
        {
            get => BeatTime;
            set => BeatTime = value;
        }

        /// <summary>
        /// Quarter notes per minute (float). Cannot be empty in the first element of the list,
        /// but otherwise it's optional, and defaults to the value of the previous element.
        /// </summary>
        [JsonPropertyName("bpm")]
        public float BeatsPerMinute { get; set; }

        /// <summary>
        /// Time signature numerator (int). Optional, defaults to 4.
        /// </summary>
        [JsonPropertyName("timeSignatureNum")]
        [PredicateIgnore]
        public int TimeSignatureNumerator { get; set; } = 4;

        /// <summary>
        /// Alias for <see cref="TimeSignatureNumerator"/>.
        /// </summary>
        public int n
        {
            get => TimeSignatureNumerator;
            set => TimeSignatureNumerator = value;
        }

        /// <summary>
        /// Time signature denominator (int). Optional, defaults to 4. Should only ever be a power of two.
        /// </summary>
        [JsonPropertyName("timeSignatureDen")]
        [PredicateIgnore]
        public int TimeSignatureDenominator { get; set; } = 4;

        /// <summary>
        /// Alias for <see cref="TimeSignatureDenominator"/>.
        /// </summary>
        public int d
        {
            get => TimeSignatureDenominator;
            set => TimeSignatureDenominator = value;
        }

        /// <summary>
        /// Beat tuplets (int[]). This defines how many steps each beat is divided into.
        /// It can either be an array of length `n` (see above) or a single integer number.
        /// Optional, defaults to `[4]`.
        /// </summary>
        [JsonPropertyName("beatTuplets")]
        [PredicateIgnore]
        public int[] BeatTuplets { get; set; } = DefaultBeatTuplets;

        /// <summary>
        /// Alias for <see cref="BeatTuplets"/>.
        /// </summary>
        public int[] bt
        {
            get => BeatTuplets;
            set => BeatTuplets = value;
        }

        /// <summary>
        /// Additional extension data.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeChange"/> class with default values.
        /// </summary>
        public TimeChange() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeChange"/> class with specified values.
        /// </summary>
        /// <param name="timeStamp">The timestamp in the specified time format.</param>
        /// <param name="bpm">The beats per minute (BPM) at this time change.</param>
        /// <param name="timeSignatureNum">The numerator of the time signature. Defaults to 4.</param>
        /// <param name="timeSignatureDen">The denominator of the time signature. Defaults to 4.</param>
        /// <param name="beatTime">The beat time. Optional.</param>
        /// <param name="beatTuplets">The beat tuplets. Optional, defaults to <see cref="DefaultBeatTuplets"/>.</param>
        public TimeChange(float timeStamp, float bpm, int timeSignatureNum = 4, int timeSignatureDen = 4, float? beatTime = null, int[]? beatTuplets = null) : this()
        {
            TimeStamp = timeStamp;
            BeatsPerMinute = bpm;
            TimeSignatureNumerator = timeSignatureNum;
            TimeSignatureDenominator = timeSignatureDen;
            BeatTime = beatTime;
            BeatTuplets = beatTuplets ?? DefaultBeatTuplets;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="TimeChange"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TimeChange"/> instance with the same data.</returns>        
        public TimeChange CloneTyped()
        {
            return new TimeChange(TimeStamp, BeatsPerMinute, TimeSignatureNumerator, TimeSignatureDenominator, BeatTime, (int[])BeatTuplets.Clone())
            {
                ExtensionData = new Dictionary<string, JsonElement>(ExtensionData ?? new Dictionary<string, JsonElement>()),
            };
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="TimeChange"/> instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return CloneTyped();
        }

        /// <summary>
        /// Produces a string representation suitable for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"SongTimeChange({TimeStamp}ms, {BeatsPerMinute}bpm)";
        }
    }
}