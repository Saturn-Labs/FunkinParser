using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Funkin.Data.Versions.v100.Chart;

namespace Funkin.Data.Converters
{
    public class LegacyNotesConverter : JsonConverter<List<Note>>
    {
        public override List<Note> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var notes = JsonSerializer.Deserialize<object[][]>(ref reader, options);
            if (notes is null)
                throw new Exception("Failed to deserialize notes.");

            List<Note> noteList = new();
            foreach (var note in notes)
            {
                if (note.Length < 3)
                    throw new Exception("Invalid note format.");
                noteList.Add(new Note
                {
                    Time = note[0] is JsonElement timeElement ? timeElement.GetSingle() : Convert.ToSingle(note[0]),
                    Data = note[1] is JsonElement dataElement ? dataElement.GetInt32() : Convert.ToInt32(note[1]),
                    Length = note[2] is JsonElement lengthElement ? lengthElement.GetSingle() : Convert.ToSingle(note[2]),
                    Parameters = note.Length > 3 && note[3] is JsonElement parametersElement ? parametersElement.Deserialize(typeof(object)) : null
                });
            }

            return noteList;
        }

        public override void Write(Utf8JsonWriter writer, List<Note> notes, JsonSerializerOptions options)
        {
            var noteList = notes.Select(n =>
                {
                    var noteObj = new List<object>
                    {
                        n.Time,
                        n.Data,
                        n.Length
                    };
                    if (n.Parameters != null)
                        noteObj.Add(n.Parameters);
                    return noteObj;
                }
            );
            JsonSerializer.Serialize(writer, noteList, options);
        }
    }
}