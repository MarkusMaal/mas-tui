using System.Text.Json;
using System.Text.Json.Serialization;

namespace MasAPI
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _serializerOptions);
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty");

            return JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            result = default(T);
            try
            {
                result = Deserialize<T>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
