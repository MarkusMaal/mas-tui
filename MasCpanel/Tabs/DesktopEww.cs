using MasCpanel.Config.Desktop;
using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class DesktopEww : TabBase
{
    private readonly EwwYuck _ewwCfg;
    private Menu? _menu;

    public DesktopEww()
    {
        Program.L.StatusText = "Eww konfiguratsiooni laadimine";
        _ewwCfg = new EwwYuck();
        _ewwCfg.LoadConfig();
        ConfigureMenu();
    }

    private void ConfigureMenu()
    {
        _menu = new Menu
        {
            ActiveColor = new Color { BackgroundColor = 7, ForegroundColor = 0 },
            DefaultColor = new Color { BackgroundColor = 16, ForegroundColor = 7 },
            MarginLeft = 1, MarginTop = 3, SelectedIndex = 0,
            TextPadding = _ewwCfg.Entries.Max(ce => ce.Tooltip.Length + 6),
        };
        
        foreach (var entry in _ewwCfg.Entries)
        {
            _menu.AddItem(entry.Tooltip, (_, _) => {}, $"Asukoht: {entry.Executable.PadRight(_ewwCfg.Entries.Max(ce => ce.Executable.Length) + 10)}\n Ikoon: {entry.Image}");
        }
    }

    public override bool VerifileOk { get; init; } = true;

    public override void ReceiveKey(object sender, ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                if (_menu?.SelectedIndex > 0)
                {
                    _menu.SelectedIndex--;
                }
                break;
            case ConsoleKey.DownArrow:
                if (_menu?.SelectedIndex < _ewwCfg.Entries.Count - 1)
                {
                    _menu.SelectedIndex++;
                }
                break;
            case ConsoleKey.Enter:
                string? newName = null;
                string? newLocation = null;
                string? newIcon = null;
                while (newName == null || newLocation == null || newIcon == null)
                {
                    Console.SetCursorPosition(4, 3 + (_menu?.SelectedIndex ?? 0));
                    var spaces = "".PadRight((_menu?.TextPadding ?? 0) - 6);
                    ColorConsole.Write($"~{_menu?.ActiveColor}{spaces}");
                    Console.SetCursorPosition(4, 3 + (_menu?.SelectedIndex ?? 0));
                    ColorConsole.Write($"~--");
                    newName = Console.ReadLine();
                    Console.SetCursorPosition(10, 4 + _ewwCfg.Entries.Count);
                    Console.Write("".PadRight(Console.WindowWidth - 10));
                    Console.SetCursorPosition(10, 4 + _ewwCfg.Entries.Count);
                    newLocation = Console.ReadLine();
                    Console.SetCursorPosition(8, 5 + _ewwCfg.Entries.Count);
                    Console.Write("".PadRight(Console.WindowWidth - 8));
                    Console.SetCursorPosition(8, 5 + _ewwCfg.Entries.Count);
                    newIcon = Console.ReadLine();
                }
                if (_menu == null) break;
                _ewwCfg.Entries[_menu.SelectedIndex] = new DesktopEntry
                {
                    Executable = newLocation,
                    Image = newIcon,
                    Tooltip = newName
                };
                ConfigureMenu();
                _ewwCfg.SaveConfig();
                break;
        }
    }

    public override void Draw(object sender, EventArgs e)
    {
        _menu?.Draw();
    }
}