using MasTUICommon.Components;
using System.Net;
using System.Text;
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
                Url = request.Url,
                Headers = new Dictionary<string, string>(),
                QueryParameters = ParseQueryString(request.Url.Query),
                Body = await ReadRequestBodyAsync(request)
            };
            // Extract headers
            foreach (string headerName in request.Headers.AllKeys)
            {
                apiRequest.Headers[headerName] = request.Headers[headerName];
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
            public static ApiResponse Ok(object data) => new ApiResponse
            {
                StatusCode = 200,
                Body = JsonHelper.Serialize(data)
            };

            public static ApiResponse Created(object data) => new ApiResponse
            {
                StatusCode = 201,
                Body = JsonHelper.Serialize(data)
            };

            public static ApiResponse NoContent() => new ApiResponse { StatusCode = 204 };

            public static ApiResponse BadRequest(string message) => new ApiResponse
            {
                StatusCode = 400,
                Body = JsonHelper.Serialize(new { error = message })
            };

            public static ApiResponse NotFound(string message = "Resource not found") => new ApiResponse
            {
                StatusCode = 404,
                Body = JsonHelper.Serialize(new { error = message })
            };

            public static ApiResponse InternalServerError(string message = "Internal server error") => new ApiResponse
            {
                StatusCode = 500,
                Body = JsonHelper.Serialize(new { error = message })
            };
        }

        public class ApiRequest
        {
            public string Method { get; set; }
            public Uri Url { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> QueryParameters { get; set; }
            public string Body { get; set; }
        }
    }
}
