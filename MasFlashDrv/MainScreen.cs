using MasFlashDrv.Config.Drives;
using MasFlashDrv.Tabs;
using MasTUICommon.Components;
using System.Reflection;

namespace MasFlashDrv
{
    internal class MainScreen
    {
        public Edition Drive { get; set; }
        private readonly TabControl? _tab;

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
        }

        public static void DrawTitleBar()
        {
            var verStr = (Assembly.GetExecutingAssembly().GetName().Version?.ToString(4)) ?? throw new NullReferenceException("Version number is undefined!");
            while (verStr.EndsWith(".0"))
            {
                verStr = verStr[..^2];
            }
            var (Left, Top) = Console.GetCursorPosition();
            Console.SetCursorPosition(0, 0);
            var prefix = "~9F";
            if (Program.C?.Status == "VERIFIED")
            {
                prefix = Config.Integration.DecodeScheme();
            }
            ColorConsole.Write(
                prefix + ($"Markuse mälupulga juhtpaneel " + verStr).PadBoth(Console.WindowWidth - 2) + " ");
            Console.SetCursorPosition(Left, Top);
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
                if (!Directory.Exists(Drive.Mount))
                {
                    Program.C?.ChooseDriveOnReload = true;
                    return;
                }
                Console.TreatControlCAsInput = false;

                if ((ck.Modifiers & ConsoleModifiers.Control) != 0 && (ck.KeyChar == 'C'))
                {
                    break;
                }
                var key = ck!.Key;
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
                        Program.ExitNow = true;
                        break;
                    case ConsoleKey.F5:
                        exit = true;
                        break;
                    default:
                        _tab!.TabItems[_tab.SelectedIndex].InvokeKeyDown(this, ck.Key);
                        // show/hide development tab
                        const string devLabel = "Arendus";
                        if (Drive.Unlocked && !_tab.TabItems.Any((p) => p.Value.Title == devLabel))
                        {
                            var devTab = new Development(Drive);
                            _tab.AddTab(new TabItem() { Title = devLabel });
                            _tab.TabItems[_tab.TabItems.Count - 1].Draw += devTab.Draw;
                            _tab.TabItems[_tab.TabItems.Count - 1].KeyDown += devTab.ReceiveKey;
                            _tab.TabItems[_tab.TabItems.Count - 1].TabEnter += (_, _) => Console.Title = "Markuse mälupulk (" + Drive.Mount + ") - " + devLabel;
                        } else if (!Drive.Unlocked && _tab.TabItems.Any((p) => p.Value.Title == devLabel))
                        {
                            _tab.TabItems.Remove(_tab.TabItems.First(p => p.Value.Title == devLabel).Key);
                        }
                        break;
                }
            }
        }
    }
}
