using MasCommon;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class CommonConfigController
    {
        private static readonly string masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        internal static async Task<ApiResponse> GetCommonConfig(ApiRequest request)
        {
            var config = new CommonConfig();
            config.Load(masRoot);
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(config, MasConfigSourceGenerationContext.Default.CommonConfig), };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> PushCommonConfig(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 400, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>() { { "Error", "Unauthorized" } }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
                return await Task.FromResult(badResponse);
            }
            var config = JsonSerializer.Deserialize(request.Body, MasConfigSourceGenerationContext.Default.CommonConfig);
            config?.Save(masRoot);
            var response = new ApiResponse { StatusCode = config != null ? 200 : 403, ContentType = "application/json", Body = JsonSerializer.Serialize(config, MasConfigSourceGenerationContext.Default.CommonConfig), };
            return await Task.FromResult(response);
        }
    }
}
