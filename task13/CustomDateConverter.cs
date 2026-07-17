using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string datePattern = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader jsonReader, Type targetType, JsonSerializerOptions serializerOptions)
        {
            var rawDateText = jsonReader.GetString();

            if (DateTime.TryParseExact(rawDateText, datePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var convertedDate))
                return convertedDate;

            throw new JsonException($"Ошибка - неверный формат. Верный - {datePattern}");
        }

        public override void Write(Utf8JsonWriter jsonWriter, DateTime sourceDateTime, JsonSerializerOptions serializerOptions)
        {
            jsonWriter.WriteStringValue(sourceDateTime.ToString(datePattern, CultureInfo.InvariantCulture));
        }
    }
}