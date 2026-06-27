using System.Text.Json.Serialization;

namespace MasAPI.Types;

public class Game
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("executable")]
    public required string Executable { get; set; }
}