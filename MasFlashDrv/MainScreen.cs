using MasFlashDrv.Config.Drives;
using MasFlashDrv.Tabs;
using MasTUICommon.Components;
using System.Reflection;

namespace MasFlashDrv
{
    internal class MainScreen
    {
        public Edition Drive { get; set; }
        private TabControl? _tab;

        public MainScreen(Edition drive)
        {
            Drive = drive;
            _tab = new TabControl()
            {
                ActiveColor = new MasTUICommon.Color() { BackgroundColor = 0xA, ForegroundColor = 0 },
                DefaultColor = new MasTUICommon.Color() { BackgroundColor = 0x8, ForegroundColor = 0 }
            };
            var tabs = new List<TabBase>();
            if (Directory.Exists(Path.Join(drive.Mount, "Markuse_videod"))) tabs.Add(new News(drive));
            if (Directory.Exists(Path.Join(drive.Mount, "markuse asjad", "markuse asjad"))) tabs.Add(new Folders(drive));
            if (Directory.Exists(Path.Join(drive.Mount, "markuse asjad", "Kiirrakendused"))) tabs.Add(new QuickApps(drive));
            tabs.Add(new Management(drive));
            tabs.Add(new Stats(drive));
            
            foreach (var tab in tabs)
            {
                var label = tab switch
                {
                    News => "Uudised",
                    QuickApps => "Kiirrakendused",
                    Folders => "Kaustad",
                    Management => "Haldamine",
                    Stats => "Statistika",
                    _ => "???"
                };
                _tab.AddTab(new TabItem { Title = label });
                _tab.TabItems[_tab.TabItems.Count - 1].Draw += tab.Draw;
                _tab.TabItems[_tab.TabItems.Count - 1].KeyDown += tab.ReceiveKey;
                _tab.TabItems[_tab.TabItems.Count - 1].TabEnter += (_, _) =>
                {
                    Console.Title = "Markuse mälupulk (" + drive.Mount + ") - " + label;
                };
            }
            Program.L.StatusText = "";
            Console.Title = "Markuse mälupulk (" + drive.Mount + ") - " + _tab.TabItems[0].Title;
            //_tab.AddTab(new TabItem { Title = "Arendamine" });
        }

        public static void DrawTitleBar()
        {
            var verStr = Assembly.GetExecutingAssembly().GetName().Version?.ToString(4);
            if (verStr == null) throw new NullReferenceException("Version number is undefined!");
            while (verStr.EndsWith(".0"))
            {
                verStr = verStr[..^2];
            }
            var backup = Console.GetCursorPosition();
            Console.SetCursorPosition(0, 0);
            ColorConsole.Write(
                $"~9F" + ($"Markuse mälupulga juhtpaneel " + verStr).PadBoth(Console.WindowWidth - 2) + " ");
            Console.SetCursorPosition(backup.Left, backup.Top);
        }

        public void Show()
        {
            bool exit = false;
            DrawTitleBar();
            while (!exit)
            {
                if (_tab == null) break;
                _tab?.Draw();
                Console.TreatControlCAsInput = true;
                var ck = Console.ReadKey(true);
                Console.TreatControlCAsInput = false;

                if (((ck.Modifiers & ConsoleModifiers.Control) != 0) && (ck.KeyChar == 'C'))
                {
                    break;
                }
                var key = ck.Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        Console.Clear();
                        DrawTitleBar();
                        if (_tab!.SelectedIndex > 0) _tab!.SelectedIndex--;
                        else _tab.SelectedIndex = _tab.TabItems.Count - 1;
                        break;
                    case ConsoleKey.RightArrow:
                        Console.Clear();
                        DrawTitleBar();
                        if (_tab!.SelectedIndex < _tab.TabItems.Count - 1) _tab!.SelectedIndex++;
                        else _tab!.SelectedIndex = 0;
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        _tab!.TabItems[_tab.SelectedIndex].InvokeKeyDown(this, ck.Key);
                        break;
                }
            }
        }
    }
}
