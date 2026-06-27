using MasAPI.Models;
using MasAPI.Types;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class MarkuStationController
    {
        internal static async Task<ApiResponse> GetConfig(ApiRequest _)
        {
            var ms = new MarkuStation();
            ms.LoadConfig();
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(ms, MarkuStationSourceGenerationContext.Default.MarkuStation), };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> GetGames(ApiRequest _)
        {
            var ms = new MarkuStation();
            ms.LoadConfig();
            var games = ms.GetGames();
            
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(games, GameArraySourceGenerationContext.Default.GameArray), };
            return await Task.FromResult(response);
        }

        internal static async Task<ApiResponse> PushConfig(ApiRequest request)
        {
            if (!(request.Headers.ContainsKey("Auth") && AuthRequest.CheckAuth(request.Headers["Auth"])))
            {
                var badResponse = new ApiResponse { StatusCode = 400, ContentType = "application/json", Body = JsonSerializer.Serialize(new Dictionary<string, string>(){ { "Error", "Unauthorized" } }, BadResponseSourceGenerationContext.Default.DictionaryStringString), };
                return await Task.FromResult(badResponse);
            }
            var msR = JsonSerializer.Deserialize(request.Body, MarkuStationSourceGenerationContext.Default.MarkuStation);
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
            var response = new ApiResponse { StatusCode = ms != null ? 200 : 403, ContentType = "application/json", Body = JsonSerializer.Serialize(ms, MarkuStationSourceGenerationContext.Default.MarkuStation), };
            return await Task.FromResult(response);
        }
    }
}
