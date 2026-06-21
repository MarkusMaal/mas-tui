using MasFlashDrv.Config.Drives;
using MasFlashDrv.Config.Stats;
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
                "basic" => 'A',
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
            Console.Write("Maht: " + SCounter.FriendlySize(CurrentDrive.FsInfo.TotalSize, true, 1) + " (" + SCounter.FriendlySize(CurrentDrive.FsInfo.TotalSize, false, 1) + ")");
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Failisüsteem: " + CurrentDrive.FsInfo.DriveFormat);
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Draiv: " + CurrentDrive.Mount);
            Console.SetCursorPosition(0, t + 1);
            Console.WriteLine("Seadistused");
            Console.WriteLine();
            new Checkbox() { Key = 'K', KeyColor = new MasTUICommon.Color() { ForegroundColor = 0xC, BackgroundColor = 0x10 }, Label = "Käivita see programm iga kord kui Markuse mälupulk on arvutisse sisestatud", Value = Program.C?.AutoRun ?? false }.Draw();
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {

        }
    }
}
