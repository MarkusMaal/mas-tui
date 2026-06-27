using MasAPI.Types;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI
{
    internal static class GlobalErrorHandlerHelpers
    {
        public static async Task<ApiResponse> HandleAsync(Exception exception)
        {
            // Log the exception (in real applications, use proper logging)
            Console.WriteLine($"Error: {exception.Message}");
            Console.WriteLine($"Stack Trace: {exception.StackTrace}");

            return exception switch
            {
                ArgumentException argEx => StatusCodes.BadRequest(argEx.Message),
                InvalidOperationException opEx => StatusCodes.BadRequest(opEx.Message),
                KeyNotFoundException => StatusCodes.NotFound(),
                JsonException => StatusCodes.BadRequest("Invalid JSON format"),
                TimeoutException => new ApiResponse
                {
                    StatusCode = 408,
                    Body = JsonSerializer.Serialize(new Dictionary<string, string>{ { "error", "Request timeout" } }, BadResponseSourceGenerationContext.Default.DictionaryStringString)
                },
                _ => StatusCodes.InternalServerError("An unexpected error occurred")
            };
        }
    }
}