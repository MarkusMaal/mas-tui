using System.Reflection;
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
    public string? VerifileStatus { get; init; }
    private string? _background;
    
    private readonly Edition _edition = new();
    
    private System.Drawing.Color? _backgroundColor;
    private System.Drawing.Color? _foregroundColor;
    public void Show()
    {
        using var colorBlockRenderer = new BrailleRenderer(new RenderOptions
        {
            MaxHeight = Console.WindowHeight - 4,
            MaxWidth = Console.WindowWidth,
        });
        _background = colorBlockRenderer.RenderFile(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "bg_common.png"));
        Program.L.StatusText = "Initializing TUI";
        var t = new TabControl
        {
            ActiveColor = new Color { BackgroundColor = 0xB, ForegroundColor = 0x0 },
            DefaultColor = new Color { BackgroundColor = 0x8, ForegroundColor = 0x0 }
        };
        if (!_edition.EditionName.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
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
        TabBase[] tabs;

        var config = new Configuration();

        if (!_edition.EditionName.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            tabs = [
                new Home(),
            new MarkuStation(),
            config,
            _edition.Features!.Contains("TS") ?
                OperatingSystem.IsLinux() &&
                File.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "eww", "eww.yuck")) &&
                Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") == "Hyprland"
                    ? new DesktopEww() : new Desktop() : new About(VerifileStatus, _edition),
            new About(VerifileStatus, _edition)
            ];
        } else
        {
            tabs = [config, new About(VerifileStatus, _edition)];
        }

        _backgroundColor = config.ColorScheme.BackgroundColor;
        _foregroundColor = config.ColorScheme.ForegroundColor;

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
        Console.SetCursorPosition(0, 1);
        if (_background != null)
        {
            Console.Write(_background);
            Console.SetCursorPosition(0, 0);
        }

        const string exitHint = "Q/Esc ";
        var verStr = Assembly.GetExecutingAssembly().GetName().Version?.ToString(4);
        if (verStr == null) throw new NullReferenceException("Version number is undefined!");
        while (verStr.EndsWith(".0"))
        {
            verStr = verStr[..^2];
        }

        var device = _edition.BuildNo[^1] switch
        {
            'a' => "arvuti",
            'b' => "virtuaalarvuti",
            'c' => "tahvelarvuti",
            'd' => "telefoni",
            'e' => "serveri",
            _ => "asjade"
        };

        var ansiBg = $"\e[48;2;{Program.Background.R};{Program.Background.G};{Program.Background.B}m";
        var ansiFg = $"\e[38;2;{Program.Foreground.R};{Program.Foreground.G};{Program.Foreground.B}m";
        ColorConsole.Write(
            $"~--{ansiBg}{ansiFg}" + ($"Markuse {device} juhtpaneel " + verStr).PadBoth(Console.WindowWidth - 2) + " ");

        Console.CursorLeft -= exitHint.Length + 1;
        ColorConsole.Write($"~--{ansiBg}\e[91m{exitHint}~--");
        Console.SetCursorPosition(0, 0);
    }
}