using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.Helpers;

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert, JsonSerializerOptions options)
        => DateTime.Parse(reader.GetString() ?? string.Empty);

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(
            value.ToUniversalTime()
                .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
}
