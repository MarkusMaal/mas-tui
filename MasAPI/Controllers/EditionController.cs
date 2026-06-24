using MasTUICommon;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    public class EditionController
    {
        internal static async Task<ApiResponse> GetEditionInfo(ApiRequest request)
        {
            var edition = new Edition();
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(edition, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), };
            return await Task.FromResult(response);
        }
    }
}
