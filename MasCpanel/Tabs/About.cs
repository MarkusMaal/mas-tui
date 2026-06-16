using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class About(string? status, Edition edition) : TabBase
{
    public override void Draw(object sender, EventArgs e)
    {
        Console.SetCursorPosition(1, 3);
        var c = edition.EditionName switch
        {
            "Pro" => new Color { BackgroundColor = 0xB },
            "Basic" or "Basic+" => new Color { BackgroundColor = 0xE },
            "Premium" => new Color { BackgroundColor = 0x1 },
            "Starter" => new Color { BackgroundColor = 0xA },
            "Ultimate" => new Color { BackgroundColor = 0xD },
            _ => new Color { BackgroundColor = 0x8 }
        };
        var s = "      ";
        ColorConsole.WriteLine($"~{c}{s}~--\n ~{c}{s}~--\n ~{c}{s}~--");
        Console.SetCursorPosition(9, 3);
        var fName = $"Markuse arvuti asjad {edition.Version} – {edition.Name}";
        Console.Write(fName);
        Console.SetCursorPosition(9, 4);
        Console.Write(edition.EditionName);
        Console.SetCursorPosition(9, 5);
        ColorConsole.WriteLine("~-8" + edition.BuildNo + "~--");
        Console.WriteLine();
        Console.WriteLine($"Keel: {edition.Language}");
        var yn = edition.Tested ? "Jah" : "Ei";
        Console.WriteLine($"Juurutatud: {yn}");
        Console.WriteLine($"Kasutaja: {edition.Username}");
        Console.WriteLine($"Kinnituskood: {edition.Pin}");
        Console.WriteLine($"Olek: {status}");
        Console.WriteLine($"Räsi: {edition.Hash[..10]}");
        var features = new Dictionary<string, string>()
        {
            { "TS", "Interaktiivne töölaud" },
            { "RM", "Rainmeter" },
            { "IP", "Integratsioonitarkvara" },
            { "CS", "Klassikaline start menüü" },
            { "MM", "Standardfunktsioonid" },
            { "RD", "Kaugjuhtimine" },
            { "WX", "Windows 10+" },
            { "LT", "LiveTuner optimeerimised" },
            { "GP", "Grupipoliitika" },
        };

        if (edition.Features == null) return;
        var marginLeft = s.Length + 10 + fName.Length;
        Console.SetCursorPosition(marginLeft, 3);
        Console.WriteLine("Funktsioonid:");
        foreach (var (i, kvp) in features.Index())
        {
            var col = edition.Features.Any(f => f == kvp.Key) ? "-" : "8";
            var pref = col == "-" ? "~-A✓~--" : "~-C✗~--";
            Console.SetCursorPosition(marginLeft, 4 + i);
            ColorConsole.WriteLine($"~-- {pref}~-{col} {kvp.Value}~--");
        }
        Console.WriteLine("\n ↵  Laadi andmed uuesti (salvestamata muudatused lähevad kaotsi)");
    }

    public override void ReceiveKey(object sender, ConsoleKey key)
    {
        if (!(sender is MainScreen mainScreen) || key != ConsoleKey.Enter)
            return;
        mainScreen.Reload();
    }
}