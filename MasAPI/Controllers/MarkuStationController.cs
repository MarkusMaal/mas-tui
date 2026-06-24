using MasAPI.Models;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class MarkuStationController
    {
        internal static async Task<ApiResponse> GetConfig(ApiRequest request)
        {
            var ms = new MarkuStation();
            ms.LoadConfig();
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(ms, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> GetGames(ApiRequest request)
        {
            var ms = new MarkuStation();
            ms.LoadConfig();
            var games = ms.GetGames();
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(games, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> PushConfig(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 400, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>(){ { "Error", "Unauthorized" } }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
                return await Task.FromResult(badResponse);
            }
            var msR = JsonSerializer.Deserialize<MarkuStation>(request.Body, JsonSerializerOptions.Default);
            var ms = new MarkuStation();
            ms.LoadConfig();
            if (msR != null)
            {
                ms.CreepypastaIntro = msR.CreepypastaIntro;
                ms.PlayIntros = msR.PlayIntros;
                ms.LegacyIntro = msR.LegacyIntro;
                ms.SpecialIntro = msR.SpecialIntro;
                ms.MonitorMode = msR.MonitorMode;
                ms.SaveConfig();
            }
            var response = new ApiResponse { StatusCode = ms != null ? 200 : 403, ContentType = "application/json", Body = JsonSerializer.Serialize(ms, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
            return await Task.FromResult(response);
        }
    }
}
