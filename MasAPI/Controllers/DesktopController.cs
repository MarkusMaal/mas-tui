using MasAPI.Types;
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

        internal static async Task<ApiResponse> SendDesktopCommand(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 401, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>() { { "Error", "Unauthorized" } }, BadResponseSourceGenerationContext.Default.DictionaryStringString), };
                return await Task.FromResult(badResponse);
            }
            var dC = JsonSerializer.Deserialize(request.Body, CommandSourceGenerationContext.Default.DesktopCommand);
            dC?.Send(masRoot);
            var response = new ApiResponse { StatusCode = dC != null ? 200 : 400, Body = JsonSerializer.Serialize(dC, CommandSourceGenerationContext.Default.DesktopCommand), ContentType = "application/json", };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> PushDesktopConfig(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 401, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>() { { "Error", "Unauthorized" } }, BadResponseSourceGenerationContext.Default.DictionaryStringString), };
                return await Task.FromResult(badResponse);
            }
            var desktopConfig = JsonSerializer.Deserialize(request.Body, DesktopLayoutSourceGenerationContext.Default.DesktopLayout);
            TextWriter tw = File.CreateText(Path.Join(masRoot, "DesktopIcons.json"));
            tw.Write(JsonSerializer.Serialize(desktopConfig, DesktopLayoutSourceGenerationContext.Default.DesktopLayout));
            tw.Close();
            tw.Dispose();
            Program.SendDesktopIconCommand("Restart");
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(desktopConfig, DesktopLayoutSourceGenerationContext.Default.DesktopLayout), };
            return await Task.FromResult(response);
        }
    }
}
