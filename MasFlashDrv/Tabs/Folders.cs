using MasFlashDrv.Config.Dirs;
using MasFlashDrv.Config.Drives;
using MasTUICommon.Components;

namespace MasFlashDrv.Tabs
{
    internal class Folders : TabBase
    {
        private Users UkssUsers { get; set; }
        private Menu UsersList { get; set; }
        private Menu? SubdirsList { get; set; }

        private readonly Dictionary<string, string> StandardDirs;

        private int SelectedStdDir { get; set; }

        internal Folders(Edition currentDrive)
        {
            SelectedStdDir = 0;
            UkssUsers = new(currentDrive);
            UsersList = new Menu()
            {
                MarginLeft = 1,
                MarginTop = 5,
                TextPadding = UkssUsers.UserDirs.Max(p => p.Name.Length) + 6,
                ActiveColor = new MasTUICommon.Color() { BackgroundColor = 7, ForegroundColor = 0 }
            };
            StandardDirs = new Dictionary<string, string>()
            {
                {"markuse asjad", "markuse asjad"},
                {"Kiirrakendused", Path.Join("markuse asjad", "Kiirrakendused")},
                {"Esiletõstetud videod", "Markuse_videod" },
                {"Abi ja info", Path.Join("markuse asjad", "Abi ja info")},
                {"Pakkfailid", "Pakkfailid"},
                {"Ventoy opsüsteemid", "multiboot"},
                {"Ventoy konfiguratsioon", "ventoy"},
                {"Ketta juurkaust", ""},
                {"Mine", Path.Join("markuse asjad", "markuse asjad", "Mine")},
            };
            var nukeDirs = new List<string>();
            foreach (var (k, v) in StandardDirs)
            {
                if (!Directory.Exists(Path.Join(currentDrive.Mount, v)))
                {
                    nukeDirs.Add(k);
                    continue;
                }
                StandardDirs[k] = Path.Join(currentDrive.Mount, v);
            }
            foreach (var d in nukeDirs)
            {
                StandardDirs.Remove(d);
            }
            foreach (var u in UkssUsers.UserDirs)
            {
                UsersList.AddItem(u.Name, (_, _) => Users.OpenDir(u), null);
            }
            GenerateSubdirsList();
        }

        private void GenerateSubdirsList()
        {
            Console.SetCursorPosition(0, 3);
            Console.Write("".PadRight(Console.WindowWidth * (Console.WindowHeight - 4)));
            var subDirs = UkssUsers.GetSubdirs(UkssUsers.UserDirs[UsersList.SelectedIndex].Name);
            SubdirsList = new Menu()
            {
                MarginLeft = UsersList.TextPadding + 5,
                MarginTop = 5,
                TextPadding = subDirs.Max(p => p.Name.Length) + 6,
                ActiveColor = new MasTUICommon.Color() { BackgroundColor = 7, ForegroundColor = 0 }
            };
            foreach (var d in subDirs)
            {
                SubdirsList.AddItem(d.Name, (_, _) => Users.OpenDir(d), null);
            }
        }

        public override void Draw(object sender, EventArgs e)
        {
            Console.WriteLine("Kasutajad");
            UsersList.Draw();
            Console.SetCursorPosition(UsersList.TextPadding + 5, 3);
            Console.WriteLine("Kaustad");
            SubdirsList?.Draw();
            Console.WriteLine();
            Console.WriteLine("Muud kaustad");
            Console.WriteLine();
            Console.CursorLeft++;
            foreach (var (i, stdDir) in StandardDirs.Index())
            {
                var arrows = i == SelectedStdDir ? "<>" : "  ";
                var fStr = $"{arrows[0]} {stdDir.Key} {arrows[1]}";
                if (Console.CursorLeft + fStr.Length > Console.WindowWidth)
                {
                    Console.WriteLine();
                    Console.CursorLeft++;
                }
                ColorConsole.Write((arrows == "<>" ? "~70" : "~--") + fStr + "~--");
                if (Console.CursorLeft + 2 < Console.WindowWidth)
                {
                    Console.CursorLeft += 2;
                } else
                {
                    Console.WriteLine();
                    Console.CursorLeft ++;
                }
            }
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    if (UsersList.SelectedIndex > 0)
                    {
                        UsersList.SelectedIndex--;
                        GenerateSubdirsList();
                    }
                    break;
                case ConsoleKey.S:
                    if (UsersList.SelectedIndex < UkssUsers.UserDirs.Length - 1)
                    {
                        UsersList.SelectedIndex++;
                        GenerateSubdirsList();
                    }
                    break;
                case ConsoleKey.D:
                    if (SelectedStdDir < StandardDirs.Count - 1) SelectedStdDir++;
                    break;
                case ConsoleKey.A:
                    if (SelectedStdDir > 0) SelectedStdDir--;
                    break;
                case ConsoleKey.UpArrow:
                    if (SubdirsList?.SelectedIndex > 0) SubdirsList.SelectedIndex--;
                    break;
                case ConsoleKey.DownArrow:
                    if (SubdirsList?.SelectedIndex < UkssUsers.GetSubdirs(UkssUsers.UserDirs[UsersList.SelectedIndex].Name).Length - 1) SubdirsList.SelectedIndex++;
                    break;
                case ConsoleKey.Enter:
                    SubdirsList?.Execute();
                    break;
                case ConsoleKey.Home:
                    UsersList.Execute();
                    break;
                case ConsoleKey.Spacebar:
                    Users.OpenDir(new DirectoryInfo(StandardDirs[StandardDirs.Keys.ToArray()[SelectedStdDir]]));
                    break;
            }
        }
    }
}
