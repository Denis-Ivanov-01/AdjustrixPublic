using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Adjustment.Project
{
    public class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        // Specify how to write the enum to JSON (serialize)
        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            // Here, we convert the enum value to a string in lowercase
            writer.WriteStringValue(value.ToString().ToLower());
        }

        // Specify how to read the enum from JSON (deserialize)
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumString = reader.GetString();

            if (string.IsNullOrEmpty(enumString))
            {
                // If null or empty, return the default value for a nullable enum
                if (Nullable.GetUnderlyingType(typeToConvert) != null)
                {
                    return default(TEnum); // This handles nullable enums
                }
            }

            // Handle both nullable and non-nullable enums
            if (Enum.IsDefined(typeof(TEnum), enumString))
            {
                // For non-nullable or nullable enums, attempt to parse the string value
                if (Enum.TryParse<TEnum>(enumString, true, out TEnum result))
                {
                    return result;
                }
            }

            throw new JsonException($"Invalid value '{enumString}' for enum {typeof(TEnum).Name}");
        }
    }
}
