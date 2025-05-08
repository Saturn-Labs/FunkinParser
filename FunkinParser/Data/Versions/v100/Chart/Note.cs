using System;
using System.Text.Json;
using Funkin.Data.Versions.Latest.Chart;
using Funkin.Utils.Interfaces;

namespace Funkin.Data.Versions.v100.Chart
{
    public class Note : ICloneable<Note>, IConvertible<NoteData>
    {
        public float Time { get; set; } = 0.0f;
        public int Data { get; set; } = 0;
        public float Length { get; set; } = 0.0f;
        public object? Parameters { get; set; } = null;
        
        public Note CloneTyped()
        {
            return new Note
            {
                Time = Time,
                Data = Data,
                Length = Length,
                Parameters = Parameters is ICloneable cloneable ? cloneable.Clone() : Parameters,
            };
        }

        public object Clone()
        {
            return CloneTyped();
        }

        public NoteData Convert()
        {
            return new NoteData
            {
                Time = Time,
                Data = Data,
                Length = Length,
                Kind = null,
                Parameters = Parameters is { } parameter ? new NoteParamData[]
                {
                    new("parameter", parameter)
                } : Array.Empty<NoteParamData>()
            };
        }

        public override string ToString()
        {
            return $"Note[LEGACY:1.0.0]({Time}, {Data}, {Length}, {(Parameters is not null ? JsonSerializer.Serialize(Parameters) : "No parameters")})";
        }
    }
}