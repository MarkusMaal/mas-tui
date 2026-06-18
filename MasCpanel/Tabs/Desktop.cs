using System.Text.Json;
using MasCommon;
using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class Desktop : TabBase
{
    private DesktopLayout? _desktopLayout;
    private Menu? _menu;
    private readonly JsonSerializerOptions _serializerOptions;

    public Desktop()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = DesktopLayoutSourceGenerationContext.Default
        };
        Program.L.StatusText = "Töölaua konfiguratsiooni laadimine";
        var mas_root = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        _desktopLayout = JsonSerializer.Deserialize<DesktopLayout>(File.ReadAllText(mas_root + "/DesktopIcons.json"), _serializerOptions);
        if (_desktopLayout == null) return;
        _menu = new Menu
        {
            ActiveColor = new Color { BackgroundColor = 7, ForegroundColor = 0 },
            DefaultColor = new Color { BackgroundColor = 16, ForegroundColor = 7 },
            MarginLeft = 1, MarginTop = 3, SelectedIndex = 0,
            TextPadding = _desktopLayout.Children.Max(ce => ce.Icon.Length + 6),
        };
        
        foreach (var entry in _desktopLayout.Children)
        {
            _menu.AddItem(entry.Icon, (_, _) => {}, $"Asukoht: {entry.Executable}");
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
                if (_menu.SelectedIndex < _desktopLayout?.Children.Length - 1)
                {
                    _menu.SelectedIndex++;
                }
                break;
        }
    }

    public override void Draw(object sender, EventArgs e)
    {
        _menu.Draw();
        Console.CursorLeft++;
        ColorConsole.WriteLine("~-CNB!~-- Markuse arvuti töölaua süsteemi redigeerimine pole selles Markuse arvuti juhtpaneeli versioonis saadaval");
    }
}