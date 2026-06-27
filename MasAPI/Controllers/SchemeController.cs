using MasAPI.Models;
using MasAPI.Types;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class SchemeController
    {
        private static string MasRoot => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        internal static async Task<ApiResponse> GetScheme(ApiRequest _)
        {
            var scheme = new Scheme();
            scheme.LoadScheme(MasRoot);
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(scheme, SchemeSourceGenerationContext.Default.Scheme), };
            return await Task.FromResult(response);
        }


        internal static async Task<ApiResponse> PushScheme(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 400, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>() { { "Error", "Unauthorized" } }, BadResponseSourceGenerationContext.Default.DictionaryStringString), };
                return await Task.FromResult(badResponse);
            }
            var dS = JsonSerializer.Deserialize(request.Body, SchemeSourceGenerationContext.Default.Scheme);
            dS?.SaveScheme(MasRoot);
            Program.SendDesktopIconCommand("ReloadTheme");
            var response = new ApiResponse { StatusCode = dS != null ? 200 : 403, ContentType = "application/json", Body = JsonSerializer.Serialize(dS, SchemeSourceGenerationContext.Default.Scheme), };

            return response;
        }
    }
}
