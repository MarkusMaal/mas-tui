using MasFlashDrv.Config.Drives;
using MasFlashDrv.Config.Stats;
using MasTUICommon.Components;

namespace MasFlashDrv.Tabs
{
    internal class Stats : TabBase
    {
        private SCounter DriveStats { get; set; }

        internal Stats(Edition currentDrive)
        {
            DriveStats = new SCounter(currentDrive);
        }

        public override void Draw(object sender, EventArgs e)
        {
            double capacity = DriveStats.TotalUsed + DriveStats.FreeSpace;
            double perc = Math.Round(DriveStats.TotalUsed / capacity * 100.0, 1);
            var maxBar = Console.WindowWidth - 50;
            ColorConsole.WriteLine("~-FKasutatud ruum~-8\t\t" + SCounter.FriendlySize(DriveStats.TotalUsed).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-F") + perc + "%~--");
            ColorConsole.WriteLine("~-FVaba ruum~-8\t\t" + SCounter.FriendlySize(DriveStats.FreeSpace).PadRight(15) + "~--");
            Console.WriteLine();
            perc = Math.Round(DriveStats.MarkusStuff / capacity * 100.0, 1);
            ColorConsole.WriteLine("~-CMarkuse asjad~-8\t\t" + SCounter.FriendlySize(DriveStats.MarkusStuff).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-C") + perc + "%~--");
            perc = Math.Round(DriveStats.BatchFiles / capacity * 100.0, 1);
            ColorConsole.WriteLine("~-APakkfailid~-8\t\t" + SCounter.FriendlySize(DriveStats.BatchFiles).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-A") + perc + "%~--");
            perc = Math.Round(DriveStats.QuickApps / capacity * 100.0, 1);
            ColorConsole.WriteLine("~-BKiirrakendused~-8\t\t" + SCounter.FriendlySize(DriveStats.QuickApps).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-B") + perc + "%~--");
            perc = Math.Round(DriveStats.OperatingSystems / capacity * 100.0, 1);
            ColorConsole.WriteLine("~-EOperatsioonsüsteemid~-8\t" + SCounter.FriendlySize(DriveStats.OperatingSystems).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-E") + perc + "%~--");
            perc = Math.Round(DriveStats.Other / capacity * 100.0, 1);
            ColorConsole.WriteLine("~-9Muu~-8\t\t\t" + SCounter.FriendlySize(DriveStats.Other).PadRight(15) + "~--" + BarGraph((int)perc, maxBar, "-9") + perc + "%~--");
        }

        private static string BarGraph(int perc, int width, string color)
        {
            if (width < 5)
            {
                return "";
            }
            var ratio = perc / 100.0 * width;
            string bar = "[~" + color;
            for (var i = 0; i < ratio; i++)
            {
                bar += "\u2588";
            }
            for (var i = ratio; i < width; i++)
            {
                bar += "\u2593";
            }
            while (bar.Length - 1 > width)
            {
                bar = bar[..^1];
            }
            return bar + "~--] ~-8";
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            // do nothing
        }
    }
}
