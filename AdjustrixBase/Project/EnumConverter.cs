using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdjustrixBase.Project
{
    public class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLower());
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumString = reader.GetString();

            if (string.IsNullOrEmpty(enumString))
            {
                if (Nullable.GetUnderlyingType(typeToConvert) != null)
                {
                    return default(TEnum);
                }
            }

            if (Enum.IsDefined(typeof(TEnum), enumString))
            {
                if (Enum.TryParse<TEnum>(enumString, true, out TEnum result))
                {
                    return result;
                }
            }

            throw new JsonException($"Invalid value '{enumString}' for enum {typeof(TEnum).Name}");
        }
    }
}
