using System.Text.Json.Serialization;

namespace MasAPI.Types
{
    // Required for generating trimmed executables
    [JsonSerializable(typeof(Dictionary<string, string>))]
    public partial class BadResponseSourceGenerationContext : JsonSerializerContext
    {
    }
}
