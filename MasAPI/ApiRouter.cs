using System.Text;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI
{
    internal class ApiRouter
    {
        private readonly Dictionary<string, Dictionary<string, Func<ApiRequest, Task<ApiResponse>>>> _routes;

        public ApiRouter()
        {
            _routes = new Dictionary<string, Dictionary<string, Func<ApiRequest, Task<ApiResponse>>>>();
        }

        public void AddRoute(string method, string pattern, Func<ApiRequest, Task<ApiResponse>> handler)
        {
            if (!_routes.ContainsKey(method))
                _routes[method] = new Dictionary<string, Func<ApiRequest, Task<ApiResponse>>>();

            _routes[method][pattern] = handler;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (var m in _routes)
            {
                foreach (var r in m.Value)
                {
                    sb.AppendLine(m.Key.PadRight(8) + r.Key);
                }
            }
            return sb.ToString();
        }

        public async Task<ApiResponse> RouteAsync(ApiRequest request)
        {
            if (!_routes.ContainsKey(request.Method))
                return new ApiResponse { StatusCode = 405, Body = JsonSerializer.Serialize(new { error = "Method not allowed" }) };

            var methodRoutes = _routes[request.Method];
            var path = request.Url.AbsolutePath;

            // Exact match first
            if (methodRoutes.ContainsKey(path))
            {
                return await methodRoutes[path](request);
            }

            // Pattern matching for parameterized routes
            foreach (var route in methodRoutes)
            {
                if (MatchesPattern(route.Key, path, out var parameters))
                {
                    request.QueryParameters = parameters;
                    return await route.Value(request);
                }
            }

            return new ApiResponse { StatusCode = 404, Body = JsonSerializer.Serialize(new { error = "Route not found" }) };
        }

        private bool MatchesPattern(string pattern, string path, out Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>();

            var patternParts = pattern.Split('/');
            var pathParts = path.Split('/');

            if (patternParts.Length != pathParts.Length) return false;

            for (int i = 0; i < patternParts.Length; i++)
            {
                if (patternParts[i].StartsWith("{") && patternParts[i].EndsWith("}"))
                {
                    var paramName = patternParts[i].Trim('{', '}');
                    parameters[paramName] = pathParts[i];
                }
                else if (patternParts[i] != pathParts[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
