using MasFlashDrv.Config.Drives;
using MasFlashDrv.Config.Stats;
using MasTUICommon.Components;

namespace MasFlashDrv.Tabs
{
    internal class Management(Edition currentDrive) : TabBase
    {
        private Edition CurrentDrive { get; init; } = currentDrive;

        private long Capacity {
            get {
                try
                {
                    return currentDrive.FsInfo.TotalSize;
                } catch (DriveNotFoundException)
                {
                    return 0;
                }
            }
        }

        private string Filesystem
        {
            get
            {
                try
                {
                    return currentDrive.FsInfo.DriveFormat;
                } catch (DriveNotFoundException)
                {
                    return "(null)";
                }
            }
        }

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
            Console.Write("Maht: " + SCounter.FriendlySize(Capacity, true, 1) + " (" + SCounter.FriendlySize(Capacity, false, 1) + ")");
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Failisüsteem: " + Filesystem);
            Console.SetCursorPosition(3 + s.Length, t++);
            Console.Write("Draiv: " + CurrentDrive.Mount);
            Console.SetCursorPosition(0, t + 1);
            Console.WriteLine("Seadistused");
            Console.WriteLine();
            if (Program.C?.Status == "VERIFIED")
            {
                Console.CursorLeft++; new Checkbox() { Key = 'K', KeyColor = new MasTUICommon.Color() { ForegroundColor = 0xC, BackgroundColor = 0x10 }, Label = "Käivita see programm iga kord kui Markuse mälupulk on arvutisse sisestatud\n", Value = Program.C?.AutoRun ?? false }.Draw();
            }
            Console.CursorLeft++; new Checkbox() { Key = 'V', KeyColor = new MasTUICommon.Color() { ForegroundColor = 0xA, BackgroundColor = 0x10 }, Label = "Vali seade andmete värskendamisel\n", Value = Program.C?.ChooseDriveOnReload ?? false }.Draw();
            Console.WriteLine();
            Console.WriteLine("Haldamine\n");
            Console.WriteLine(" F5  - Laadi andmed uuesti");
            Console.WriteLine(" Esc - Välju");
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.K:
                    Program.C?.AutoRun = !Program.C.AutoRun;
                    break;
                case ConsoleKey.V:
                    Program.C?.ChooseDriveOnReload = !Program.C.ChooseDriveOnReload;
                    break;
            }
        }
    }
}
