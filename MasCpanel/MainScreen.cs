using ConsoleImage.Core;
using MasCpanel.Tabs;
using MasTUICommon;
using MasTUICommon.Components;
using Color = MasTUICommon.Color;
using Configuration = MasCpanel.Tabs.Configuration;

namespace MasCpanel;

public class MainScreen
{
    private bool _reload;
    public string? VerifileStatus { get; set; }
    private string? _background;
    
    private readonly Edition _edition = new();
    public void Show()
    {
        using var colorBlockRenderer = new BrailleRenderer(new RenderOptions
        {
            MaxHeight = Console.WindowHeight - 1,
            MaxWidth = Console.WindowWidth,
        });
        _background = colorBlockRenderer.RenderFile(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "bg_common.png"));
        Program.L.StatusText = "Initializing TUI";
        var t = new TabControl
        {
            ActiveColor = new Color { BackgroundColor = 0xB, ForegroundColor = 0x0 },
            DefaultColor = new Color { BackgroundColor = 0x8, ForegroundColor = 0x0 }
        };
        if (!_edition.EditionName.ToLower().StartsWith("basic"))
        {
            t.AddTab(new TabItem { Title = "Skriptid" });
            t.AddTab(new TabItem { Title = "MarkuStation" });
        }
        t.AddTab(new TabItem { Title = "Konfiguratsioon" });
        if (_edition.Features!.Contains("TS"))
        {
            t.AddTab(new TabItem { Title = "Töölaud" });
        }
        t.AddTab(new TabItem { Title = "Teave" });
        TabBase[] tabs = [];

        if (!_edition.EditionName.ToLower().StartsWith("basic"))
        {
            tabs = [
                new Home(),
            new MarkuStation(),
            new Configuration(),
            _edition.Features!.Contains("TS") ?
                OperatingSystem.IsLinux() &&
                File.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "eww", "eww.yuck")) &&
                Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") == "Hyprland"
                    ? new DesktopEww() : new Desktop() : new About(VerifileStatus, _edition),
            new About(VerifileStatus, _edition)
            ];
        } else
        {
            tabs = [new Configuration(), new About(VerifileStatus, _edition)];
        }

        foreach (var (i, tab) in tabs.Index())
        {
            if (i > t.TabItems.Count - 1) break;
            t.TabItems[i].Draw += tab.Draw;
            t.TabItems[i].KeyDown += tab.ReceiveKey;
        }

        Program.L.StatusText = "";
        Cls();
        while (true)
        {
            t.Draw();
            var consoleBreak = false;
            var key = Console.ReadKey().Key; 
            switch (key)
            {
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    Console.Clear();
                    consoleBreak = true;
                    break;
                case ConsoleKey.RightArrow:
                    Cls();
                    if (t.SelectedIndex < t.TabItems.Count - 1)
                    {
                        t.SelectedIndex++;
                    }
                    else
                    {
                        t.SelectedIndex = 0;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    Cls();
                    if (t.SelectedIndex > 0)
                    {
                        t.SelectedIndex--;
                    }
                    else
                    {
                        t.SelectedIndex = t.TabItems.Count - 1;
                    }
                    break;
                default:
                    t.TabItems[t.SelectedIndex].InvokeKeyDown(this, key);
                    break;
            }

            if (consoleBreak || _reload) break;
        }
    }

    public void Reload()
    {
        Cls();
        _reload = true;
        new Thread(Program.Reload).Start();
    }

    public void Cls()
    {
        Console.Clear();
        if (_background == null) return;
        Console.Write(_background);
        Console.SetCursorPosition(0, 0);
    }
}