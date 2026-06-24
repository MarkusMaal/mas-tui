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
                    return CurrentDrive.FsInfo.TotalSize;
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
                    return CurrentDrive.FsInfo.DriveFormat;
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
            Console.WriteLine(" F2  - " + (!CurrentDrive.Unlocked ? "Ava mälupulk haldamiseks  " : "Lukusta haldusfunktsioonid"));
            Console.WriteLine(" F3  - Muuda PIN koodi");
            Console.WriteLine(" F4  - Lülita ebaturvaline PIN kood " + (CurrentDrive.IsLegacyPinEnabled ? "välja" : "sisse"));
            Console.WriteLine(" F5  - Laadi andmed uuesti");
            Console.WriteLine(" F6  - Varundushaldur");
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
                case ConsoleKey.F2:
                    CurrentDrive.Unlocked = !CurrentDrive.Unlocked && CurrentDrive.CheckPin(PinEntry.ShowDialog("Sisesta PIN kood"));
                    break;
                case ConsoleKey.F3:
                    if (CurrentDrive.Unlocked || CurrentDrive.CheckPin(PinEntry.ShowDialog("Sisesta vana PIN kood")))
                    {
                        var newPin = PinEntry.ShowDialog("Sisesta uus PIN kood");
                        TextWriter tw;
                        if (CurrentDrive.IsLegacyPinEnabled)
                        {
                            tw = File.CreateText(Path.Join(CurrentDrive.Mount, "NTFS", "config.sys"));
                            tw.WriteLine(newPin);
                            tw.Close();
                            tw.Dispose();
                        }
                        tw = File.CreateText(Path.Join(CurrentDrive.Mount, "NTFS", "spin.sys"));
                        tw.WriteLine(Edition.GenerateSecurePin(newPin));
                        tw.Close();
                        tw.Dispose();
                        CurrentDrive.ReloadPins();
                    }
                    else
                    {
                        ShowMsg.ShowDialog("Vale PIN kood");
                    }
                    break;
                case ConsoleKey.F4:
                    if (CurrentDrive.Unlocked || CurrentDrive.CheckPin(PinEntry.ShowDialog("Sisesta vana PIN kood")))
                    {
                        var setPin = "";
                        if (!CurrentDrive.IsLegacyPinEnabled)
                        {
                            setPin = PinEntry.ShowDialog("Sisesta uus PIN kood");
                        }
                        var tw = File.CreateText(Path.Join(CurrentDrive.Mount, "NTFS", "config.sys"));
                        tw.Write(CurrentDrive.IsLegacyPinEnabled ? "Ebaturvaline PIN kood keelatud\nInsecure authentication code disabled\n" : $"{setPin}\n");
                        tw.Close();
                        tw.Dispose();
                        if (CurrentDrive.Pin != null)
                        {
                            tw = File.CreateText(Path.Join(CurrentDrive.Mount, "NTFS", "spin.sys"));
                            tw.WriteLine(Edition.GenerateSecurePin(setPin));
                            tw.Close();
                            tw.Dispose();
                        }
                        CurrentDrive.ReloadPins();
                    }
                    else
                    {
                        ShowMsg.ShowDialog("Vale PIN kood");
                    }
                    break;
                case ConsoleKey.F6:
                    if (CurrentDrive.Unlocked || CurrentDrive.CheckPin(PinEntry.ShowDialog("Sisesta PIN kood")))
                    {
                        var bm = new BackupManager(CurrentDrive);
                        bm.Show();
                    }
                    else
                    {
                        ShowMsg.ShowDialog("Vale PIN kood");
                    }
                    break;
            }
        }
    }
}
