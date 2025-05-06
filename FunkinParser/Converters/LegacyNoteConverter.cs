using System;
using System.Collections.Generic;
using Funkin.Core.Data.v10X;
using Newtonsoft.Json;

namespace Funkin.Converters
{
    public class LegacyNoteConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Note[]);
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value is Note[] notes)
            {
                foreach (var note in notes)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(note.Time);
                    writer.WriteValue(note.StrumType);
                    writer.WriteValue(note.Length);
                    if (note.CustomData != null)
                        writer.WriteValue(note.CustomData);
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var noteList = new List<Note>();

            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonSerializationException("Expected StartArray token");

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                if (reader.TokenType != JsonToken.StartArray)
                    throw new JsonSerializationException("Expected StartArray for inner note array");

                var time = 0f;
                var strumType = 0;
                var length = 0f;
                object? customData = null;
                
                if (!reader.Read() || reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer)
                    throw new JsonSerializationException("Invalid or missing 'time' in note array");
                time = Convert.ToSingle(reader.Value);

                if (!reader.Read() || reader.TokenType != JsonToken.Integer)
                    throw new JsonSerializationException("Invalid or missing 'strumType' in note array");
                strumType = Convert.ToInt32(reader.Value);

                if (!reader.Read() || reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer)
                    throw new JsonSerializationException("Invalid or missing 'length' in note array");
                length = Convert.ToSingle(reader.Value);

                if (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    customData = reader.Value;
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }
                }

                noteList.Add(new Note
                {
                    Time = time,
                    StrumType = strumType,
                    Length = length,
                    CustomData = customData
                });
            }

            return noteList.ToArray();
        }
    }
}