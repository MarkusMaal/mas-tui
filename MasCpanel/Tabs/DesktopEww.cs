using MasCpanel.Config.Desktop;
using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class DesktopEww : TabBase
{
    private EwwYuck _ewwCfg;
    private Menu _menu;

    public DesktopEww()
    {
        Program.L.StatusText = "Loading eww configuration";
        _ewwCfg = new EwwYuck();
        _ewwCfg.LoadConfig();
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
    
    public override void ReceiveKey(object sender, ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                if (_menu.SelectedIndex > 0)
                {
                    _menu.SelectedIndex--;
                }
                break;
            case ConsoleKey.DownArrow:
                if (_menu.SelectedIndex < _ewwCfg.Entries.Count - 1)
                {
                    _menu.SelectedIndex++;
                }
                break;
        }
    }

    public override void Draw(object sender, EventArgs e)
    {
        _menu.Draw();
    }
}