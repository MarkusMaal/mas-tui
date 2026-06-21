using MasFlashDrv.Config.Drives;
using MasTUICommon.Components;

namespace MasFlashDrv
{
    internal abstract class DrivePicker()
    {
        public static void Show()
        {
            Edition[] drives = [.. Program.fDf!.Drives];
            Menu m = new()
            {
                MarginLeft = 1,
                MarginTop = 4,
                TextPadding = drives.Max(d => GetFormattedLabel(d).Length) + 1,
                ActiveColor = new MasTUICommon.Color { BackgroundColor = 7, ForegroundColor = 0 }
            };
            foreach (var d in drives)
            {
                m.AddItem(GetFormattedLabel(d), (_, _) => { new MainScreen(d).Show(); }, null);
            }
            var stop = false;
            var topLabel = "Vali mälupulk";
            while (!stop)
            {
                MainScreen.DrawTitleBar();
                Console.ResetColor();
                if (drives.Length == 1)
                {
                    m.SelectedIndex = 0;
                    var onceOk = true;
                    while (onceOk || (!Program.ExitNow && !Program.C.ChooseDriveOnReload))
                    {
                        m.Execute();
                        onceOk = false;
                    }
                    Console.Clear();
                    Program.fDf = new();
                    drives = [.. Program.fDf!.Drives];
                    m.Clear();
                    foreach (var d in drives)
                    {
                        m.AddItem(GetFormattedLabel(d), (_, _) => { new MainScreen(d).Show(); }, null);
                    }
                    m.SelectedIndex = 0;
                    Console.Clear();
                    if (Program.ExitNow) break;
                    continue;
                }
                else
                {
                    Console.SetCursorPosition(0, 2);
                    Console.WriteLine(topLabel);
                    m.Draw();
                }
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        if (m.SelectedIndex > 0) m.SelectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (m.SelectedIndex < drives.Length - 1) m.SelectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        Console.SetCursorPosition(0, 2);
                        Console.WriteLine("".PadRight(topLabel.Length));
                        var onceOk = true;
                        while (onceOk || (!Program.ExitNow && !Program.C.ChooseDriveOnReload))
                        {
                            m.Execute();
                            onceOk = false;
                        }
                        Console.Clear();
                        Program.fDf = new();
                        drives = [.. Program.fDf!.Drives];
                        m.Clear();
                        foreach (var d in drives)
                        {
                            m.AddItem(GetFormattedLabel(d), (_, _) => { new MainScreen(d).Show(); }, null);
                        }
                        m.SelectedIndex = 0;
                        Console.Clear();
                        MainScreen.DrawTitleBar();
                        m.Draw();
                        break;
                    case ConsoleKey.Escape:
                        stop = true;
                        break;
                }
                if (Program.ExitNow) break;
            }
        }

        private static string GetFormattedLabel(Edition drive)
        {
            var label = drive.FsInfo.VolumeLabel;
            if (label == "")
            {
                label = "(sildita)";
            }
            return drive.Mount.PadRight(15) + label.PadRight(15) + Config.Stats.SCounter.FriendlySize(drive.FsInfo.TotalSize) + " ";
        }
    }
}
