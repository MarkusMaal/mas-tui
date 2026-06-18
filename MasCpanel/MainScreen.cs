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
    
    private TabControl? _tab;
    public void Show()
    {
        using var colorBlockRenderer = new BrailleRenderer(new RenderOptions
        {
            MaxHeight = Console.WindowHeight - 4,
            MaxWidth = Console.WindowWidth,
        });
        _background = colorBlockRenderer.RenderFile(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "bg_common.png"));
        Program.L.StatusText = "TUI ettevalmistamine";
        _tab = new TabControl
        {
            ActiveColor = new Color { BackgroundColor = 0xB, ForegroundColor = 0x0 },
            DefaultColor = new Color { BackgroundColor = 0x8, ForegroundColor = 0x0 }
        };
        if (!_edition.EditionName.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            _tab.AddTab(new TabItem { Title = "Skriptid" });
            _tab.AddTab(new TabItem { Title = "MarkuStation" });
        }
        _tab.AddTab(new TabItem { Title = "Konfiguratsioon" });
        if (_edition.Features!.Contains("TS"))
        {
            _tab.AddTab(new TabItem { Title = "Töölaud" });
        }
        _tab.AddTab(new TabItem { Title = "Teave" });
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

        foreach (var (i, tab) in tabs.Index())
        {
            if (i > _tab.TabItems.Count - 1) break;
            _tab.TabItems[i].Draw += tab.Draw;
            _tab.TabItems[i].KeyDown += tab.ReceiveKey;
        }

        Program.L.StatusText = "";
        Cls();
        while (true)
        {
            _tab.Draw();
            var consoleBreak = false;
            Console.TreatControlCAsInput = true;
            var keyInfo = Console.ReadKey(true);
            Console.TreatControlCAsInput = false;
            
            if (((keyInfo.Modifiers & ConsoleModifiers.Control) != 0) && (keyInfo.KeyChar == 'C'))
            {
                break;
            }
            var key = keyInfo.Key; 
            switch (key)
            {
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    Console.Clear();
                    consoleBreak = true;
                    break;
                case ConsoleKey.RightArrow:
                    Cls();
                    if (_tab.SelectedIndex < _tab.TabItems.Count - 1)
                    {
                        _tab.SelectedIndex++;
                    }
                    else
                    {
                        _tab.SelectedIndex = 0;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    Cls();
                    if (_tab.SelectedIndex > 0)
                    {
                        _tab.SelectedIndex--;
                    }
                    else
                    {
                        _tab.SelectedIndex = _tab.TabItems.Count - 1;
                    }
                    break;
                case ConsoleKey.C:
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        consoleBreak = true;
                    }
                    break;
                default:
                    _tab.TabItems[_tab.SelectedIndex].InvokeKeyDown(this, key);
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

        if (Console.CursorLeft > exitHint.Length) Console.CursorLeft -= exitHint.Length + 1;
        ColorConsole.Write($"~--{ansiBg}\e[91m{exitHint}~--");
        Console.SetCursorPosition(0, 0);
    }
}