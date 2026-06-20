using MasFlashDrv.Config.Drives;
using MasTUICommon.Components;

namespace MasFlashDrv
{
    internal abstract class DrivePicker()
    {
        public static void Show(Edition[] drives)
        {
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
                Console.SetCursorPosition(0, 2);
                Console.WriteLine(topLabel);
                m.Draw();
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
                        m.Execute();
                        Console.Clear();
                        break;
                    case ConsoleKey.Escape:
                        stop = true;
                        break;
                }
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
