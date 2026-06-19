using System.Diagnostics;
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

    private List<string> _desktopIcons = [];

    public sealed override bool VerifileOk { get; init; }

    public Desktop()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = DesktopLayoutSourceGenerationContext.Default
        };
        Program.L.StatusText = "Failide olemasolu kontrollimine";
        VerifileOk = Program.CheckFiles(Verifile.FileScope.DesktopIcons);
        if (!VerifileOk) return;
        Program.L.StatusText = "Töölaua konfiguratsiooni laadimine";
        var mas_root = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        _desktopLayout = JsonSerializer.Deserialize<DesktopLayout>(File.ReadAllText(mas_root + "/DesktopIcons.json"), _serializerOptions);
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
        }
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
        ColorConsole.WriteLine("~-CNB!~-- Markuse arvuti töölaua süsteemi redigeerimine pole selles Markuse arvuti juhtpaneeli versioonis saadaval");
        var offsetX = (_desktopLayout?.Children.Max(c => c.Icon.Length) ?? 0) + 18;
        var offsetY = 3;
        Console.SetCursorPosition(offsetX, offsetY++);
        Console.Write("Ikoone saadaval: " + _desktopIcons.Count);
        var selectedItem = _desktopLayout!.Children[_menu.SelectedIndex];
        Console.SetCursorPosition(offsetX, offsetY++);
        Console.Write($"Asukoht: {selectedItem.LocationX}x{selectedItem.LocationY}    ");
        Console.SetCursorPosition(offsetX, offsetY++);
        Console.Write($"Ruudustik: {_desktopLayout.IconCountX}x{_desktopLayout.IconCountY}");
        Console.SetCursorPosition(offsetX, offsetY++);
        Console.Write($"Veeris: {_desktopLayout.IconPadding}");
        Console.SetCursorPosition(offsetX, offsetY++);
        Console.Write($"Ikooni suurus: {_desktopLayout.IconSize}");
        offsetY++;
        Console.SetCursorPosition(offsetX, offsetY++);
        new Checkbox {Value = _desktopLayout.LockIcons, KeyColor = new Color {ForegroundColor = 0xC}, Label = "Lukusta ikoonid", Key = 'L'}.Draw();
        Console.CursorLeft += 3;
        new Checkbox {Value = _desktopLayout.ShowActions, KeyColor = new Color {ForegroundColor = 0xA}, Label = "Kuva juhtnupud", Key = 'j'}.Draw();
        Console.SetCursorPosition(offsetX, offsetY++);
        new Checkbox {Value = _desktopLayout.ShowIcons, KeyColor = new Color {ForegroundColor = 0xB}, Label = "Kuva ikoonid", Key = 'i'}.Draw();
        Console.CursorLeft += 6;
        new Checkbox {Value = _desktopLayout.ShowLogo, KeyColor = new Color {ForegroundColor = 0xE}, Label = "Kuva logo", Key = 'g'}.Draw();
        Console.SetCursorPosition(offsetX, offsetY);
    }
}