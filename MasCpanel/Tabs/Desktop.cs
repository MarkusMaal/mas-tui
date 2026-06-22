using MasCommon;
using MasTUICommon;
using MasTUICommon.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace MasCpanel.Tabs;

public class Desktop : TabBase
{
    private readonly DesktopLayout? _desktopLayout;
    private Menu? _menu;

    private readonly List<string> _desktopIcons = [];

    public sealed override bool VerifileOk { get; init; }

    public Desktop()
    {
        Program.L.StatusText = "Failide olemasolu kontrollimine";
        VerifileOk = Program.CheckFiles(Verifile.FileScope.DesktopIcons);
        if (!VerifileOk) return;
        Program.L.StatusText = "Töölaua konfiguratsiooni laadimine";
        var mas_root = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        _desktopLayout = JsonSerializer.Deserialize(File.ReadAllText(mas_root + "/DesktopIcons.json"), DesktopLayoutSourceGenerationContext.Default.DesktopLayout);
        Program.L.StatusText = "Töölaua ikoonide tuvastamine";
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = mas_root + "/Markuse asjad/DesktopIcons" + (OperatingSystem.IsMacOS() ? ".app/Contents/MacOS/DesktopIcons" : "") +
                           (OperatingSystem.IsWindows() ? ".exe" : ""),
                Arguments = "--icons",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            }
        };
        proc.Start();
        _desktopIcons.Clear();
        while (!proc.StandardOutput.EndOfStream)
        {
            var line = proc.StandardOutput.ReadLine();
            if (string.IsNullOrEmpty(line)) continue;
            _desktopIcons.Add(line);
            Program.L.StatusText = "Tuvastatud ikoon: " + line;
        }
        if (_desktopLayout == null) return;
        ReloadMenu();
    }

    private void ReloadMenu()
    {
        _menu = new Menu
        {
            ActiveColor = new Color { BackgroundColor = 7, ForegroundColor = 0 },
            DefaultColor = new Color { BackgroundColor = 16, ForegroundColor = 7 },
            MarginLeft = 1, MarginTop = 3, SelectedIndex = 0,
            TextPadding = _desktopLayout!.Children.Max(ce => ce.Icon.Length + 6),
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
                if (_menu?.SelectedIndex > 0)
                {
                    _menu.SelectedIndex--;
                }
                break;
            case ConsoleKey.DownArrow:
                if (_menu?.SelectedIndex < _desktopLayout?.Children.Length - 1)
                {
                    _menu.SelectedIndex++;
                }
                break;
            case ConsoleKey.I:
                _desktopLayout?.ShowIcons = !_desktopLayout.ShowIcons;
                Program.SendDesktopIconCommand("IsIconVisible", _desktopLayout?.ShowIcons ?? false ? "true" : "false");
                break;
            case ConsoleKey.J:
                _desktopLayout?.ShowActions = !_desktopLayout.ShowActions;
                Program.SendDesktopIconCommand("IsActionVisible", _desktopLayout?.ShowActions ?? false ? "true" : "false");
                break;
            case ConsoleKey.G:
                _desktopLayout?.ShowLogo = !_desktopLayout.ShowLogo;
                Program.SendDesktopIconCommand("IsLogoVisible", _desktopLayout?.ShowLogo ?? false ? "true" : "false");
                break;
            case ConsoleKey.L:
                _desktopLayout?.LockIcons = !_desktopLayout.LockIcons;
                Program.SendDesktopIconCommand("Lock", _desktopLayout?.LockIcons ?? false ? "true" : "false");
                break;
            case ConsoleKey.T:
                CloseIcons();
                ShowIcons();
                break;
            case ConsoleKey.R:
                var newDims = RenderTextbox("Uued ruudustiku mõõtmed")?.Split('x');
                if (newDims == null || _desktopLayout == null) break;
                try
                {
                    var x = newDims[0];
                    var y = newDims[1];
                    _desktopLayout.IconCountX = int.Parse(x);
                    _desktopLayout.IconCountY = int.Parse(y);
                    SaveDesktopSettings();
                    ReloadIcons();
                } catch (IndexOutOfRangeException) {}
                break;
            case ConsoleKey.V:
                var newPadding = RenderTextbox("Uus veeris");
                if (newPadding == null || _desktopLayout == null) break;
                try
                {
                    _desktopLayout.IconPadding = int.Parse(newPadding);
                    SaveDesktopSettings();
                    ReloadIcons();
                } catch (IndexOutOfRangeException) {}
                break;
            case ConsoleKey.S:
                var newSize = RenderTextbox("Uus suurus");
                if (newSize == null || _desktopLayout == null) break;
                try
                {
                    _desktopLayout.IconSize = int.Parse(newSize);
                    SaveDesktopSettings();
                    ReloadIcons();
                } catch (IndexOutOfRangeException) {}
                break;
            case ConsoleKey.Enter:
                var newLocation = RenderTextbox("Uus asukoht");
                if (newLocation == null || _desktopLayout == null || _menu == null) break;
                _desktopLayout.Children[_menu.SelectedIndex].Executable = newLocation;
                Console.WriteLine("Saadavalolevad ikoonid: " + string.Join(", ", _desktopIcons));
                var newIcon = RenderTextbox("Uus ikoon: ");
                Console.CursorTop -= 1;
                Console.Write("".PadRight(Console.WindowWidth));
                if (newIcon == null) break;
                _desktopLayout.Children[_menu.SelectedIndex].Icon = newIcon;
                SaveDesktopSettings();
                ReloadIcons();
                ReloadMenu();
                break;
            case ConsoleKey.O:
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "DesktopIcons.json"),
                    }
                };
                p.Start();
                break;
        }
    }

    private static void CloseIcons()
    {
        foreach (var p in Process.GetProcessesByName("DesktopIcons" +
                                                     (OperatingSystem.IsWindows() ? ".exe" : "")))
        {
            p.Kill();
        }
    }

    private void SaveDesktopSettings()
    {
        var masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        var jsonData = JsonSerializer.Serialize(_desktopLayout, DesktopLayoutSourceGenerationContext.Default.DesktopLayout);
        File.WriteAllText(masRoot + "/DesktopIcons.json", jsonData, encoding: Encoding.UTF8);
    }
    
    private static void ReloadIcons()
    {
        Program.SendDesktopIconCommand("Restart", "true");
    }

    private static void ShowIcons()
    {

        var masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        var p2 = new Process
        {
            StartInfo = {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = masRoot + "/Markuse asjad/DesktopIcons" + (OperatingSystem.IsWindows() ? ".exe" : ""),
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                    
            }
        };
        // some additional nonsense is required if we're not in Windows 
        if (!OperatingSystem.IsWindows())
        {
            p2.StartInfo.Arguments = "-c \"nohup '" + p2.StartInfo.FileName + "' > /dev/null 2>&1 &\"";
            p2.StartInfo.FileName = "bash";
        }
        p2.Start();
    }

    private static string? RenderTextbox(string label)
    {
        var originalLeft = Console.CursorLeft;
        Console.CursorLeft = Console.WindowWidth - 2;
        Console.Write("]");
        Console.CursorLeft = originalLeft;
        Console.Write(label + ": [");
        var r = Console.ReadLine();
        Console.CursorTop--;
        Console.CursorLeft = 0;
        Console.Write("".PadRight(Console.WindowWidth));
        Console.CursorLeft = 0;
        return r;
    }

    public override void Draw(object sender, EventArgs e)
    {
        if (!VerifileOk)
        {
            Console.WriteLine("Puuduvad vajalikud failid töölauaikoonide seadistamiseks!\nNõuanne: Käivita DesktopIcons vähemalt üks kord enne juhtpaneeli avamist.");
            return;
        }
        _menu?.Draw();
        if (_menu == null) return;
        Console.CursorLeft++;
        ColorConsole.WriteLine("~--JS(~-2O~--)N redigeerimine");
        var offsetX = (_desktopLayout?.Children.Max(c => c.Icon.Length) ?? 0) + 18;
        var offsetY = 3;
        Console.SetCursorPosition(offsetX, offsetY++);
        var selectedItem = _desktopLayout!.Children[_menu.SelectedIndex];
        Console.Write($"Asukoht: {selectedItem.LocationX}x{selectedItem.LocationY}    ");
        Console.SetCursorPosition(offsetX, offsetY++);
        ColorConsole.Write("~--(~-1T~--)aaskäivita töölauaikoonid");
        Console.SetCursorPosition(offsetX, offsetY++);
        ColorConsole.Write($"~--(~-DR~--)uudustik: {_desktopLayout.IconCountX}x{_desktopLayout.IconCountY}");
        Console.SetCursorPosition(offsetX, offsetY++);
        ColorConsole.Write($"~--(~-FV~--)eeris: {_desktopLayout.IconPadding}");
        Console.SetCursorPosition(offsetX, offsetY++);
        ColorConsole.Write($"~--Ikooni (~-9s~--)uurus: {_desktopLayout.IconSize}");
        offsetY++;
        Console.SetCursorPosition(offsetX, offsetY++);
        new Checkbox {Value = _desktopLayout.LockIcons, KeyColor = new Color {ForegroundColor = 0xC}, Label = "Lukusta ikoonid", Key = 'L'}.Draw();
        Console.CursorLeft += 3;
        new Checkbox {Value = _desktopLayout.ShowActions, KeyColor = new Color {ForegroundColor = 0xA}, Label = "Kuva juhtnupud", Key = 'j'}.Draw();
        Console.SetCursorPosition(offsetX, offsetY++);
        new Checkbox {Value = _desktopLayout.ShowIcons, KeyColor = new Color {ForegroundColor = 0xB}, Label = "Kuva ikoonid", Key = 'i'}.Draw();
        Console.CursorLeft += 6;
        new Checkbox {Value = _desktopLayout.ShowLogo, KeyColor = new Color {ForegroundColor = 0xE}, Label = "Kuva logo", Key = 'g'}.Draw();
        Console.SetCursorPosition(0, offsetY + 4);
    }
}