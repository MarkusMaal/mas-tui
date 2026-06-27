using MasAPI.Types;
using MasTUICommon.Components;
using System.Net;
using System.Text.Json;
using static MasAPI.MasAPIServer;

namespace MasAPI
{
    internal abstract class RequestParser
    {
        public static async Task<ApiRequest> ParseAsync(HttpListenerRequest request)
        {
            var methodCol = request.HttpMethod switch
            {
                "POST" => "E",
                "GET" => "A",
                "PUT" => "9",
                "PATCH" => "D",
                "DELETE" => "C",
                _ => "F"
            };
            ColorConsole.WriteLine($"~-8[{DateTime.Now:yyy-MM-dd HH:mm:ss}] ~-{methodCol}{request.HttpMethod}~-- -> {request.Url}");
            var apiRequest = new ApiRequest
            {
                Method = request.HttpMethod,
                Url = request.Url ?? new Uri(""),
                Headers = [],
                QueryParameters = ParseQueryString(request.Url?.Query ?? ""),
                Body = await ReadRequestBodyAsync(request)
            };
            // Extract headers
            foreach (var headerName in request.Headers.AllKeys)
            {
                if (headerName == null) continue;
                apiRequest.Headers[headerName] = request.Headers[headerName] ?? "";
            }
            return apiRequest;
        }

        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(queryString)) return parameters;

            queryString = queryString.TrimStart('?');
            var pairs = queryString.Split('&');

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    parameters[Uri.UnescapeDataString(keyValue[0])] =
                        Uri.UnescapeDataString(keyValue[1]);
                }
            }

            return parameters;
        }

        private static async Task<string> ReadRequestBodyAsync(HttpListenerRequest request)
        {
            if (request.ContentLength64 == 0) return string.Empty;

            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            return await reader.ReadToEndAsync();
        }

        // Common status code helpers
        public static class StatusCodes
        {
            public static ApiResponse Ok(Dictionary<string, string> data) => new()
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(data, BadResponseSourceGenerationContext.Default.DictionaryStringString)
            };

            public static ApiResponse Created(Dictionary<string, string> data) => new()
            {
                StatusCode = 201,
                Body = JsonSerializer.Serialize(data, BadResponseSourceGenerationContext.Default.DictionaryStringString)
            };

            public static ApiResponse NoContent() => new() { StatusCode = 204 };

            public static ApiResponse BadRequest(string message) => new()
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new Dictionary<string, string> { { "error", message } }, BadResponseSourceGenerationContext.Default.DictionaryStringString)
            };

            public static ApiResponse NotFound(string message = "Resource not found") => new()
            {
                StatusCode = 404,
                Body = JsonSerializer.Serialize(new Dictionary<string, string> { { "error", message } }, BadResponseSourceGenerationContext.Default.DictionaryStringString)
            };

            public static ApiResponse InternalServerError(string message = "Internal server error") => new()
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new Dictionary<string, string> { { "error", message } }, BadResponseSourceGenerationContext.Default.DictionaryStringString)
            };
        }

        public class ApiRequest
        {
            public required string Method { get; set; }
            public required Uri Url { get; set; }
            public required Dictionary<string, string> Headers { get; set; }
            public required Dictionary<string, string> QueryParameters { get; set; }
            public required string Body { get; set; }
        }
    }
}
