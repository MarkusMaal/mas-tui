using System.Text.Json.Serialization;

namespace MasAPI.Models
{
    // Required for generating trimmed executables
    [JsonSerializable(typeof(Scheme))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(int))]
    public partial class SchemeSourceGenerationContext : JsonSerializerContext
    {
    }
}
