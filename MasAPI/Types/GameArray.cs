using System.Text.Json.Serialization;

namespace MasAPI.Types
{
    // Required for generating trimmed executables
    [JsonSerializable(typeof(Game[]))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(int))]
    public partial class GameArraySourceGenerationContext : JsonSerializerContext
    {
    }
}
