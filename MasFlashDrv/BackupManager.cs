using MasFlashDrv.Config.Backups;
using MasFlashDrv.Config.Drives;
using MasTUICommon;
using MasTUICommon.Components;
using System.Diagnostics;

namespace MasFlashDrv
{
    internal class BackupManager
    {
        private BackupFinder? _backupFinder;
        private Menu? _menu;
        private Config.Drives.Edition CurrentDrive { get; init; }

        internal BackupManager(Config.Drives.Edition currentDrive)
        {
            CurrentDrive = currentDrive;
            Refresh();
        }
        
        private void Refresh()
        {
            Console.Clear();
            MainScreen.DrawTitleBar();
            Console.ResetColor();
            Program.L.StatusText = "Varukoopiate avastamine...";
            _backupFinder = new BackupFinder();
            Program.L.StatusText = "";
            _menu = new()
            {
                ActiveColor = new Color() { BackgroundColor = 7, ForegroundColor = 0 },
                TextPadding = _backupFinder.Backups.Max(p => p.ToString().Length) + 6,
                MarginTop = 9,
                MarginLeft = 1
            };
            foreach (var backup in _backupFinder.Backups)
            {
                _menu.AddItem(backup.ToString(), (_, _) => { }, null);
            }
        }

        private static void SaveAndShowTxt(string[] lines)
        {
            var tmp = Path.ChangeExtension(Path.GetTempFileName(), "txt");
            File.WriteAllText(tmp, string.Join(OperatingSystem.IsWindows() ? "\r\n" : "\n", lines));
            var p = new Process()
            {
                StartInfo =
                            {
                                FileName = tmp,
                                UseShellExecute = true
                            }
            };
            p.Start();
            p.WaitForExit();
            File.Delete(tmp);
        }

        public void Show()
        {
            if (_backupFinder == null) return;
            Console.Clear();
            var exit = false;
            Dictionary<string, string> keyTips = new()
            {
                { "F1", "Ava asukohas" },
                { "F2", "Sõltuvused" },
                { "F3", "Kõik failid" },
                { "F4", "Muuda nime" },
                { "F5", "Laadi uuesti" },
                { "Insert", "Varunda" },
                { "DEL", "Kustuta" },
                { "Enter", "Taasta" },
                { "Esc", "Välju" },
            };
            while (!exit)
            {
                MainScreen.DrawTitleBar();
                Console.ResetColor();
                Console.SetCursorPosition(0, 2);
                ColorConsole.WriteLine("~-B\u2191\u2193~-- Varundamine ja taaste");
                Console.WriteLine();
                Console.WriteLine("Mälupulk: " + CurrentDrive.Mount);
                Console.WriteLine("Varukoopiate asukoht: " + _backupFinder.RootDirectory);
                Console.WriteLine();
                ColorConsole.WriteLine("~--(~-AJ~--)uuruta varukoopia teisele seadmele");
                Console.WriteLine();
                _menu?.Draw();

                var lines = 1;
                var x = 0;
                foreach (var kvp in keyTips)
                {
                    if (x > Console.WindowWidth - kvp.Key.Length - kvp.Value.Length - 6)
                    {
                        lines++;
                        x = 0;
                    }
                    x += 6 + kvp.Key.Length + kvp.Value.Length;
                }

                Console.SetCursorPosition(0, Console.WindowHeight - lines);
                foreach (var kvp in keyTips)
                {
                    if (Console.CursorLeft >  Console.WindowWidth - kvp.Key.Length - kvp.Value.Length - 6)
                    {
                        Console.WriteLine();
                    }
                    ColorConsole.Write($"~70 {kvp.Key} ~-- {kvp.Value}   ");
                }
                if (_menu == null) return;
                var selectedBackup = _backupFinder.Backups[_menu.SelectedIndex];
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        Console.Clear();
                        exit = true;
                        break;
                    case ConsoleKey.DownArrow:
                        if (_menu.SelectedIndex < _backupFinder.Backups.Count - 1) _menu.SelectedIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (_menu.SelectedIndex > 0) _menu.SelectedIndex++;
                        break;
                    case ConsoleKey.F1:
                        new Process()
                        {
                            StartInfo =
                            {
                                FileName = selectedBackup.Location,
                                UseShellExecute = true
                            }
                        }.Start();
                        break;
                    case ConsoleKey.F2:
                        SaveAndShowTxt(selectedBackup.MissingFiles);
                        break;
                    case ConsoleKey.F3:
                        SaveAndShowTxt(selectedBackup.AllFiles);
                        break;
                    case ConsoleKey.F4:
                        var name = Backupname.ShowDialog();
                        selectedBackup.Rename(name);
                        Refresh();
                        break;
                    case ConsoleKey.F5:
                        Refresh();
                        break;
                    case ConsoleKey.Delete:
                        selectedBackup.Delete();
                        Refresh();
                        break;
                    case ConsoleKey.Insert:
                        _ = Backupname.ShowDialog();
                        break;
                }
            }
        }
    }
}
