using MasAPI.Controllers;
using MasTUICommon;
using System.Net;
using System.Text;
using static MasAPI.RequestParser;

namespace MasAPI
{
    internal class MasAPIServer
    {
        private HttpListener? _listener;
        private readonly ApiRouter _router;
        private readonly string _baseUrl = "http://+:14415/";

        public MasAPIServer()
        {
            _router = new ApiRouter();
            ConfigureRoutes();
        }

        private void ConfigureRoutes()
        {
            _router.AddRoute("GET", "/config", CommonConfigController.GetCommonConfig);
            _router.AddRoute("GET", "/desktop", DesktopController.GetDesktopConfig);
            _router.AddRoute("GET", "/edition", EditionController.GetEditionInfo);
            _router.AddRoute("GET", "/markustation/config", MarkuStationController.GetConfig);
            _router.AddRoute("GET", "/markustation/games", MarkuStationController.GetGames);
            _router.AddRoute("GET", "/scheme", SchemeController.GetScheme);
            _router.AddRoute("POST", "/config", CommonConfigController.PushCommonConfig);
            _router.AddRoute("POST", "/markustation/config", MarkuStationController.PushConfig);
        }

        public async Task StartAsync()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_baseUrl);
            _listener.Start();
            Console.Clear();
            Utils.CheckCodepage();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   \u25CF");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  \u25CF ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\u25CF");
            Console.ResetColor();
            Console.WriteLine("  markuse asjad API");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("   \u25CF");
            Console.ResetColor();

            Console.Write("\nAPI server started at ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_baseUrl);
            Console.ResetColor();
            Console.WriteLine($"\nAvailable routes:");
            Console.WriteLine(_router);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"Press Ctrl+C to cancel...\r");
            Console.ResetColor();

            await ListenForRequestsAsync();
        }

        private async Task ListenForRequestsAsync()
        {
            while (_listener?.IsListening ?? false)
            {
                var context = await _listener.GetContextAsync();
                _ = ProcessRequestAsync(context);
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            ApiRequest apiRequest = null;
            if (DataController.SendDataIfApplicable(context)) return;
            var request = context.Request;
            apiRequest = await ParseAsync(context.Request);

            var response = await _router.RouteAsync(apiRequest);

            await SendResponseAsync(context.Response, response);
        }

        private async Task SendResponseAsync(HttpListenerResponse response1, ApiResponse response2)
        {
            response1.StatusCode = response2.StatusCode;
            response1.ContentType = response2.ContentType;

            // Add custom headers
            foreach (var header in response2.Headers)
            {
                response1.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(response2.Body))
            {
                var buffer = Encoding.UTF8.GetBytes(response2.Body);
                response1.ContentLength64 = buffer.Length;
                await response1.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }

            response1.Close();
        }

        public class ApiResponse
        {
            public int StatusCode { get; set; } = 200;
            public string ContentType { get; set; } = "application/json";
            public string Body { get; set; } = string.Empty;
            public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        }
    }
}
