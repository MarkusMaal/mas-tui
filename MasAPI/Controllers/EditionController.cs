using MasTUICommon;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;
using static MasTUICommon.Edition;

namespace MasAPI.Controllers
{
    public class EditionController
    {
        internal static async Task<ApiResponse> GetEditionInfo(ApiRequest _)
        {
            var edition = new Edition();
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(edition, EditionSourceGenerationContext.Default.Edition), };
            return await Task.FromResult(response);
        }
    }
}
