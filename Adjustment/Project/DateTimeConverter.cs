using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdjustrixBase.Project
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string _dateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        public DateTimeConverter()
        {

        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateFormat));
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateStr = reader.GetString();
            return DateTime.ParseExact(dateStr, _dateFormat, null);
        }
    }
}
