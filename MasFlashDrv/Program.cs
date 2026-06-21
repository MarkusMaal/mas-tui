using MasFlashDrv.Config;
using MasFlashDrv.Config.Drives;
using MasTUICommon;
using System.Diagnostics;

namespace MasFlashDrv
{
    internal partial class Program
    {
        public static readonly Loader L = new();
        public static Config.News.Feed F = new();
        public static Integration? C;
        private static FlashDriveFinder? fDf;

        static async Task Main(string[] args)
        {
            Console.Title = "Markuse mälupulk";
            Console.CancelKeyPress += (_, e) =>
            {
                if (e.SpecialKey != ConsoleSpecialKey.ControlC) return;
                if (e.SpecialKey == ConsoleSpecialKey.ControlBreak) Process.GetCurrentProcess().Kill();
                Console.Clear();
                Console.Error.WriteLine("Rakenduse ohutu peatamine nurjus");
            };
            L.StatusTextChanged += LoadCheck;
            new Thread(SpinLoader).Start();
            L.StatusText = "Ettevalmistamine";
            await F.Read();
            fDf = new();
            C = new();
            L.StatusText = "";
            Console.Clear();
            if (fDf.Drives.Count == 1)
            {
                new MainScreen(fDf.Drives[0]).Show();
            } else
            {
                DrivePicker.Show([.. fDf.Drives]);
            }
            Console.Clear();
        }


        public static void SpinLoader()
        {
            Console.CursorVisible = false;
            while (true)
            {
                L.StatusText = L.StatusText;
                Thread.Sleep(1);
                if (L.StatusText == "") break;
            }
            Console.CursorVisible = true;
        }

        private static void LoadCheck(string status)
        {
            if (status != "")
            {
                L.PrintLoader();
            }
            else
            {
                Console.Write("".PadRight(Console.WindowWidth - 1) + "\r");
            }
        }
    }
}
