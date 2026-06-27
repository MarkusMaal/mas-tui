using MasCommon;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class DesktopController
    {
        private static readonly string masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        internal static async Task<ApiResponse> GetDesktopConfig(ApiRequest _)
        {
            var jsonFile = Path.Join(masRoot, "DesktopIcons.json");
            var json = await File.ReadAllTextAsync(jsonFile);
            var desktop = JsonSerializer.Deserialize(json, DesktopLayoutSourceGenerationContext.Default.DesktopLayout);
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(desktop, DesktopLayoutSourceGenerationContext.Default.DesktopLayout), };
            return await Task.FromResult(response);
        }
    }
}
