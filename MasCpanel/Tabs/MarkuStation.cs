using MasCpanel.Config.MarkuStation;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class MarkuStation : TabBase
{
    private int SelectedGame { get; set; } = 0;
    public override void ReceiveKey(object sender, ConsoleKey key)
    {
        Console.Write("\r   ");
        switch (key)
        {
            case ConsoleKey.DownArrow:
                SelectedGame++;
                break;
            case ConsoleKey.UpArrow:
                SelectedGame--;
                break;
            case ConsoleKey.L:
                Program.MsConfig.PlayIntros = !Program.MsConfig.PlayIntros;
                break;
            case ConsoleKey.C:
                Program.MsConfig.CreepypastaIntro = !Program.MsConfig.CreepypastaIntro;
                break;
            case ConsoleKey.P:
                Program.MsConfig.LegacyIntro = !Program.MsConfig.LegacyIntro;
                break;
            case ConsoleKey.S:
                Program.MsConfig.SpecialIntro = !Program.MsConfig.SpecialIntro;
                break;
            case ConsoleKey.M:
                var i = (int)Program.MsConfig.MonitorMode;
                i++;
                if (i >= 4) i = 0;
                Program.MsConfig.MonitorMode = (MonitorMode)i;
                break;
            case ConsoleKey.R:
                Program.MsConfig.SaveConfig();
                break;
            case ConsoleKey.Enter:
                if (Program.MsConfig.GetGames().Length != 0)
                {
                    EditGame();
                }

                break;
            case ConsoleKey.U:
                AddGame();
                break;
            case ConsoleKey.Delete:
                if (Program.MsConfig.GetGames().Length != 0)
                {
                    Program.MsConfig.DeleteGame(SelectedGame);
                    CleanTable();
                }
                break;
        }

        if (SelectedGame < 0) SelectedGame = 0;
        else if (SelectedGame > Program.MsConfig.GetGames().Length - 1) SelectedGame = Program.MsConfig.GetGames().Length - 1;
    }

    private void EditGame()
    {
        var (name, location) = GameEditor();
        Program.MsConfig.EditGame(SelectedGame, name, location);
    }

    private void AddGame()
    {
        var (name, location) = GameEditor();
        Program.MsConfig.AddGame(new Game
        {
            Name = name,
            Executable = location,
        });
    }

    private (string name, string location) GameEditor()
    {
        string? name = null;
        string? loc = null;
        while (name == null || loc == null || !File.Exists(loc) || name.Length < 1)
        {
            Console.SetCursorPosition(0, 11);
            Console.WriteLine("".PadRight(Console.WindowWidth * 4));
            Console.SetCursorPosition(0, 11);
            Console.Write("Nimi: ");
            name = Console.ReadLine();
            Console.Write("Asukoht: ");
            loc = Console.ReadLine();
        }
        return (name, loc);
    }

    private void CleanTable()
    {
        Console.SetCursorPosition(0, 3);
        Console.Write("".PadRight(Console.WindowWidth * 7));
    }
    
    public override void Draw(object sender, EventArgs e)
    {
        Console.SetCursorPosition(0, 3);
        const string label = "Mängud";
        const int minWidth = 62;
        var maxWidth = Program.MsConfig.GetGames().Length > 0 ? Math.Max(
            Program.MsConfig.GetGames().Max(g => g.Name.Length) + 23 +
            Program.MsConfig.GetGames().Max(g => g.Executable.Length), minWidth) : minWidth;
        Console.WriteLine($"\u2554\u2550{label}".PadRight(maxWidth - 3 - label.Length, '\u2550') +
                          "\u2557");
        Game[] displayList = [];
        if (Program.MsConfig.GetGames().Length != 0)
        {
            var padding = Program.MsConfig.GetGames().Max(g => g.Name.Length) + 4;
            var skip = Math.Min(SelectedGame - 4, Program.MsConfig.GetGames().Length - 5);
            if (SelectedGame < 4) skip = 0;
            displayList = Program.MsConfig.GetGames().Skip(skip).Take(5).ToArray();
            foreach (var (index, game) in displayList.Index())
            {
                var rowLabel = game.Name.PadRight(padding) + game.Executable;
                var arrows = "  ";
                if ((skip == 0 || displayList.Length < 5) && index == SelectedGame )
                {
                    arrows = "<>";
                }
                else if (displayList.Length == 5 && (skip < Program.MsConfig.GetGames().Length - 5))
                {
                    if (skip != 0 && 4 == index)
                    {
                        arrows = "<>";
                    }
                }
                else if (displayList.Length == 5 && (SelectedGame == Program.MsConfig.GetGames().Length - 5 + index))
                {
                    arrows = "<>";
                }

                var col = arrows == "<>" ? "~70" : "~--";
                ColorConsole.WriteLine($"~--\u2551 {col}{arrows[0]} " + rowLabel.PadRight(maxWidth - 16) +
                                       $" {arrows[1]}~-- \u2551");
            }
        }

        if (displayList.Length < 5)
        {
            for (var i = 0; i < 5 - displayList.Length; i++)
            {
                Console.WriteLine($"\u2551   " + "-----Tühi-----".PadRight(maxWidth - 16) +
                                  $"   \u2551");
            }
        }
        var hint = " U Lisa mäng   DEL Kustuta   \u21B5 Muuda   \u2195 Vali mäng ";
        Console.WriteLine($"\u255A".PadRight(maxWidth - 10 - hint.Length, '\u2550') +
                          $"{hint}\u2550\u255D");
        Console.WriteLine();

        var monitorMode = Program.MsConfig.MonitorMode switch
        {
            MonitorMode.Internal => "Ainult peamine ekraan",
            MonitorMode.External => "Ainult teine ekraan",
            MonitorMode.Extend => "Laienda",
            MonitorMode.Clone => "Klooni",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Console.WriteLine($" [{PrintCheck(Program.MsConfig.PlayIntros)}] (L)uba introd\t\t(M)onitor režiim: {monitorMode}".PadRight(maxWidth / 2));
        Console.WriteLine($" [{PrintCheck(Program.MsConfig.CreepypastaIntro)}] (C)reepypasta intro\t(R)akenda muudatused");
        Console.WriteLine($" [{PrintCheck(Program.MsConfig.LegacyIntro)}] (P)ärandintro");
        Console.WriteLine($" [{PrintCheck(Program.MsConfig.SpecialIntro)}] (S)pecial intro");
    }

    private string PrintCheck(bool val)
    {
        return (val ? "X" : " ");
    }
}