using MasCommon;
using MasTUICommon.Components;

namespace MasAPI
{
    internal class Program
    {
        public static Verifile? vf;
        static async Task Main()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                ColorConsole.WriteLine($"~-8[{DateTime.Now:yyy-MM-dd HH:mm:ss}]~-- Shutting down server");
                e.Cancel = false;
                Thread.Sleep(500);
                Environment.Exit(0);
            };
            Console.Write("Checking for verifile tamper...");
            if (!Verifile.CheckVerifileTamper())
            {
                Console.WriteLine("Verifile tamper verification failed");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" OK");
            Console.ResetColor();
            vf = new();
            var status = vf.MakeAttestation();
            if (status != "VERIFIED")
            {
                Console.WriteLine("Verifile attestation failed\nStatus: VF_" + status);
                return;
            }
            var server = new MasAPIServer();
            await server.StartAsync();
        }

        public static void SendDesktopIconCommand(string type, string args = "")
        {
            DesktopCommand cmd = new()
            {
                Arguments = args,
                Type = type
            };
            cmd.Send(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas"));
        }
    }
}
