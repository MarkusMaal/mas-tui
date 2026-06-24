using System.Net;

namespace MasAPI.Controllers
{
    internal class DataController
    {
        private static readonly string masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        public static bool SendDataIfApplicable(HttpListenerContext context)
        {
            switch (context.Request.RawUrl)
            {
                case "/favicon.ico":
                    SendFile(context, Path.Join(masRoot, "mas.ico"));
                    return true;
                default:
                    if ((context.Request.RawUrl?.StartsWith("/mas") ?? false) && File.Exists(Path.Join(masRoot, context.Request.RawUrl[4..])) && context.Request.RawUrl.EndsWith(".png"))
                    {
                        SendFile(context, Path.Join(masRoot, context.Request.RawUrl[4..]));
                        return true;
                    }
                    break;
            }
            return false;
        }

        private static void SendFile(HttpListenerContext context, string fileName)
        {
            context.Response.OutputStream.Write(File.ReadAllBytes(fileName));
            context.Response.Close();
        }
    }
}
