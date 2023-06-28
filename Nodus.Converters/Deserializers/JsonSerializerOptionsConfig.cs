using Nodus.Converters.JsonTypeConverters;
using System.Text.Json;

namespace Nodus.Converters.Deserializers
{
    public static class JsonSerializerOptionsConfig
    {
        public static JsonSerializerOptions ConfigureJsonSerializerOptions(this JsonSerializerOptions options)
        {
            options.PropertyNameCaseInsensitive = true;
            options.MaxDepth = 64;
            options.Converters.Add(new DateTimeConverter());

            return options;
        }
    }
}
