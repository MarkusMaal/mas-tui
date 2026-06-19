using MasFlashDrv.Config.Drives;
using MasTUICommon.Components;

namespace MasFlashDrv.Tabs
{
    internal class Management(Edition currentDrive) : TabBase
    {
        private Edition CurrentDrive { get; init; } = currentDrive;

        public override void Draw(object sender, EventArgs e)
        {
            Console.Write($"Teave");
            var c = CurrentDrive.EditionName.ToLower() switch
            {
                "basic" => 'E',
                "premium" => 'C',
                "ultimate" => 'D',
                _ => '8'
            };
            var s = "            ";
            Console.SetCursorPosition(1, 5);
            ColorConsole.WriteLine($"~{c}-{s}~--\n ~{c}-{s}~--\n ~{c}-{s}~--\n ~{c}-{s}~--\n ~{c}-{s}~--");
            var t = 5;
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Markuse mälupulk");
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Väljaanne: " + CurrentDrive.EditionName);
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Maht: " + Math.Round(CurrentDrive.FsInfo.TotalSize / 1000.0 / 1000.0 / 1000.0, 1) + "GB (" + Math.Round(CurrentDrive.FsInfo.TotalSize / 1024.0 / 1024.0 / 1024.0, 1) + " GiB)");
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Failisüsteem: " + CurrentDrive.FsInfo.DriveFormat);
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Draiv: " + CurrentDrive.Mount);
            Console.SetCursorPosition(0, t + 1);
            Console.WriteLine("Seadistused");
            Console.WriteLine("N/A");
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {

        }
    }
}
