using System.Text.Json.Serialization.Metadata;

namespace MasAPI.Models
{
    public interface ISchemeSourceGenerationContext
    {
        JsonTypeInfo? GetTypeInfo(Type type);
    }
}