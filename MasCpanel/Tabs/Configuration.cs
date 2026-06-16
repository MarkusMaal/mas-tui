using System.Drawing;
using ConsoleImage.Core;
using MasCommon;
using MasCpanel.Config;
using MasTUICommon.Components;
using Color = MasTUICommon.Color;

namespace MasCpanel.Tabs;

public class Configuration : TabBase
{

  private readonly string _desktopPreview;
  private readonly string _uncommonPreview;
  private readonly string _loginPreview;
  private readonly CommonConfig _config;
  private readonly string _masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
  private readonly Checkbox _logoCheck = new()
  {
    Key = 'K',
    KeyColor = new Color
    {
      BackgroundColor = 16 /*0x10*/,
      ForegroundColor = 10
    },
    Label = "Kuva Markuse asjade logo käivitumisel"
  };
  private readonly Checkbox _scheduleCheck = new()
  {
    Key = 'a',
    KeyColor = new Color
    {
      BackgroundColor = 16 /*0x10*/,
      ForegroundColor = 11
    },
    Label = "Luba ajastatud sündmused"
  };
  private readonly Checkbox _desktopCheck = new Checkbox
  {
    Key = 't',
    KeyColor = new Color
    {
      BackgroundColor = 16 /*0x10*/,
      ForegroundColor = 14
    },
    Label = "Käivita töölauamärkmed käivitumisel"
  };
  private readonly Scheme _colorScheme = new();

  public Configuration()
  {
    Program.L.StatusText = "Generating previews";
    using var colorBlockRenderer = new ColorBlockRenderer(new RenderOptions
    {
      MaxHeight = 5
    });
    _desktopPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_desktop.png"));
    _uncommonPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_uncommon.png"));
    _loginPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_login.png"));
    Program.L.StatusText = "Loading integration configuration";
    _config = new CommonConfig();
    _config.Load(_masRoot);
    _logoCheck.Value = _config.ShowLogo;
    _desktopCheck.Value = _config.AutostartNotes;
    _scheduleCheck.Value = _config.AllowScheduledTasks;
    Program.L.StatusText = "Loading color scheme";
    _colorScheme.LoadScheme(_masRoot);
  }

  public override void ReceiveKey(object sender, ConsoleKey key)
  {
    switch (key)
    {
      case ConsoleKey.A:
        _config.AllowScheduledTasks = !_config.AllowScheduledTasks;
        _config.Save(_masRoot);
        _scheduleCheck.Value = _config.AllowScheduledTasks;
        break;
      case ConsoleKey.E:
        EditForeground();
        break;
      case ConsoleKey.K:
        _config.ShowLogo = !_config.ShowLogo;
        _config.Save(_masRoot);
        _logoCheck.Value = _config.ShowLogo;
        break;
      case ConsoleKey.T:
        _config.AutostartNotes = !_config.AutostartNotes;
        _config.Save(_masRoot);
        _desktopCheck.Value = _config.AutostartNotes;
        break;
      case ConsoleKey.U:
        EditBackground();
        break;
      case ConsoleKey.Add:
      case ConsoleKey.OemPlus:
        _config.PollRate += 5;
        _config.Save(_masRoot);
        break;
      case ConsoleKey.Subtract:
      case ConsoleKey.OemMinus:
        _config.PollRate -= 5;
        _config.Save(_masRoot);
        break;
    }
    Console.WriteLine("\r   ");
  }

  private (int, int) GetAnsiDims(string ansi)
  {
    return (ansi.Split('\n')[0].Split("\u001B[0m").Length, ansi.Split('\n').Length);
  }

  private void ShowAnsiAtPosition(string ansi, int x, int y)
  {
    foreach ((int Index, string Item) in ansi.Split('\n').Index<string>())
    {
      Console.SetCursorPosition(x, y + Index);
      Console.Write(Item);
    }
    Console.ResetColor();
  }

  private void EditBackground()
  {
    Console.SetCursorPosition(50, 13);
    Console.Write("Taust:  \t#      ");
    Console.CursorLeft -= 6;
    _colorScheme.BackgroundColor = ColorTranslator.FromHtml("#" + Console.ReadLine());
    _colorScheme.SaveScheme(_masRoot);
  }

  private void EditForeground()
  {
    Console.SetCursorPosition(50, 14);
    Console.Write("Esiplaan:  \t#      ");
    Console.CursorLeft -= 6;
    _colorScheme.ForegroundColor = ColorTranslator.FromHtml("#" + Console.ReadLine());
    _colorScheme.SaveScheme(_masRoot);
  }

  public override void Draw(object sender, EventArgs e)
  {
    Console.WriteLine("Taustad:");
    (int num1, int _) = GetAnsiDims(_desktopPreview);
    (int num2, int _) = GetAnsiDims(_loginPreview);
    Console.SetCursorPosition(1, 5);
    ColorConsole.Write("~--Töölaud (~-D1~--)");
    ShowAnsiAtPosition(_desktopPreview, 1, 6);
    Console.SetCursorPosition(2 + num1, 5);
    ColorConsole.Write("~--Logimisekraan (~-F2~--)");
    ShowAnsiAtPosition(_loginPreview, 2 + num1, 6);
    Console.SetCursorPosition(4 + num1 + num2, 5);
    ColorConsole.Write("~--Miniversioon (~-93~--)");
    ShowAnsiAtPosition(_uncommonPreview, 4 + num1 + num2, 6);
    Console.SetCursorPosition(10, 3);
    ColorConsole.WriteLine("~--(~-CV~--)aheta miniversioon töölauataustaga");
    Console.SetCursorPosition(1, 12);
    Console.WriteLine("Integratsioon:");
    Console.CursorLeft = 2;
    _logoCheck.Draw();
    ++Console.CursorTop;
    Console.CursorLeft = 2;
    _scheduleCheck.Draw();
    ++Console.CursorTop;
    Console.CursorLeft = 2;
    _desktopCheck.Draw();
    Console.WriteLine();
    ColorConsole.WriteLine($"~--  < Pollimise sagedus: {_config.PollRate.ToString()}ms (~-A+~--/~-C-~--) >");
    Console.SetCursorPosition(50, 12);
    Console.WriteLine("Värvid:");
    Console.SetCursorPosition(50, 13);
    ColorConsole.WriteLine("~--Ta(~-4u~--)st:\t" + _colorScheme.BackgroundToHexString());
    Console.SetCursorPosition(50, 14);
    ColorConsole.WriteLine("~--(~-2E~--)siplaan:\t" + _colorScheme.ForegroundToHexString());
  }
}