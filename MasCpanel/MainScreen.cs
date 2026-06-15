using System.Runtime.CompilerServices;
using MasCommon;
using MasCpanel.Tabs;
using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel;

public class MainScreen
{
    public string? VerifileStatus { get; set; }
    public void Show()
    {
        Program.L.StatusText = "Initializing TUI";
        var t = new TabControl
        {
            ActiveColor = new Color { BackgroundColor = 0xB, ForegroundColor = 0x0 },
            DefaultColor = new Color { BackgroundColor = 0x8, ForegroundColor = 0x0 }
        };
        t.AddTab(new TabItem { Title = "Avaleht" });
        t.AddTab(new TabItem { Title = "MarkuStation" });
        t.AddTab(new TabItem { Title = "Teave" });

        TabBase[] tabs = [new Home(), new MarkuStation(), new About(VerifileStatus)];

        foreach (var (i, tab) in tabs.Index())
        {
            t.TabItems[i].Draw += tab.Draw;
            t.TabItems[i].KeyDown += tab.ReceiveKey;
        }

        Program.L.StatusText = "";
        Console.Clear();
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
                    Console.Clear();
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
                    Console.Clear();
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

            if (consoleBreak) break;
        }
    }
}