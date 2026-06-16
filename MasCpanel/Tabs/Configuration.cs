using System.Drawing;
using ConsoleImage.Core;
using MasCommon;
using MasCpanel.Config;
using MasTUICommon.Components;

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
    KeyColor = new MasTUICommon.Color()
    {
      BackgroundColor = 16 /*0x10*/,
      ForegroundColor = 10
    },
    Label = "Kuva Markuse asjade logo käivitumisel"
  };
  private readonly Checkbox _scheduleCheck = new()
  {
    Key = 'a',
    KeyColor = new MasTUICommon.Color()
    {
      BackgroundColor = 16 /*0x10*/,
      ForegroundColor = 11
    },
    Label = "Luba ajastatud sündmused"
  };
  private readonly Checkbox _desktopCheck = new Checkbox()
  {
    Key = 't',
    KeyColor = new MasTUICommon.Color()
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
    using (ColorBlockRenderer colorBlockRenderer = new ColorBlockRenderer(new RenderOptions()
    {
      MaxHeight = 5
    }))
    {
      this._desktopPreview = colorBlockRenderer.RenderFile(Path.Join(this._masRoot, "bg_desktop.png"));
      this._uncommonPreview = colorBlockRenderer.RenderFile(Path.Join(this._masRoot, "bg_uncommon.png"));
      this._loginPreview = colorBlockRenderer.RenderFile(Path.Join(this._masRoot, "bg_login.png"));
      Program.L.StatusText = "Loading integration configuration";
      this._config = new CommonConfig();
      this._config.Load(this._masRoot);
      this._logoCheck.Value = this._config.ShowLogo;
      this._desktopCheck.Value = this._config.AutostartNotes;
      this._scheduleCheck.Value = this._config.AllowScheduledTasks;
      Program.L.StatusText = "Loading color scheme";
      this._colorScheme.LoadScheme(this._masRoot);
    }
  }

  public override void ReceiveKey(object sender, ConsoleKey key)
  {
    switch (key)
    {
      case ConsoleKey.A:
        this._config.AllowScheduledTasks = !this._config.AllowScheduledTasks;
        this._config.Save(this._masRoot);
        this._scheduleCheck.Value = this._config.AllowScheduledTasks;
        break;
      case ConsoleKey.E:
        this.EditForeground();
        break;
      case ConsoleKey.K:
        this._config.ShowLogo = !this._config.ShowLogo;
        this._config.Save(this._masRoot);
        this._logoCheck.Value = this._config.ShowLogo;
        break;
      case ConsoleKey.T:
        this._config.AutostartNotes = !this._config.AutostartNotes;
        this._config.Save(this._masRoot);
        this._desktopCheck.Value = this._config.AutostartNotes;
        break;
      case ConsoleKey.U:
        this.EditBackground();
        break;
      case ConsoleKey.Add:
      case ConsoleKey.OemPlus:
        this._config.PollRate += 5;
        this._config.Save(this._masRoot);
        break;
      case ConsoleKey.Subtract:
      case ConsoleKey.OemMinus:
        this._config.PollRate -= 5;
        this._config.Save(this._masRoot);
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
    foreach ((int Index, string Item) in ((IEnumerable<string>) ansi.Split('\n')).Index<string>())
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
    this._colorScheme.BackgroundColor = ColorTranslator.FromHtml("#" + Console.ReadLine());
    this._colorScheme.SaveScheme(this._masRoot);
  }

  private void EditForeground()
  {
    Console.SetCursorPosition(50, 14);
    Console.Write("Esiplaan:  \t#      ");
    Console.CursorLeft -= 6;
    this._colorScheme.ForegroundColor = ColorTranslator.FromHtml("#" + Console.ReadLine());
    this._colorScheme.SaveScheme(this._masRoot);
  }

  public override void Draw(object sender, EventArgs e)
  {
    Console.WriteLine("Taustad:");
    (int num1, int _) = this.GetAnsiDims(this._desktopPreview);
    (int num2, int _) = this.GetAnsiDims(this._loginPreview);
    Console.SetCursorPosition(1, 5);
    ColorConsole.Write("~--Töölaud (~-D1~--)");
    this.ShowAnsiAtPosition(this._desktopPreview, 1, 6);
    Console.SetCursorPosition(2 + num1, 5);
    ColorConsole.Write("~--Logimisekraan (~-F2~--)");
    this.ShowAnsiAtPosition(this._loginPreview, 2 + num1, 6);
    Console.SetCursorPosition(4 + num1 + num2, 5);
    ColorConsole.Write("~--Miniversioon (~-93~--)");
    this.ShowAnsiAtPosition(this._uncommonPreview, 4 + num1 + num2, 6);
    Console.SetCursorPosition(10, 3);
    ColorConsole.WriteLine("~--(~-CV~--)aheta miniversioon töölauataustaga");
    Console.SetCursorPosition(1, 12);
    Console.WriteLine("Integratsioon:");
    Console.CursorLeft = 2;
    this._logoCheck.Draw();
    ++Console.CursorTop;
    Console.CursorLeft = 2;
    this._scheduleCheck.Draw();
    ++Console.CursorTop;
    Console.CursorLeft = 2;
    this._desktopCheck.Draw();
    Console.WriteLine();
    ColorConsole.WriteLine($"~--  < Pollimise sagedus: {this._config.PollRate.ToString()}ms (~-A+~--/~-C-~--) >");
    Console.SetCursorPosition(50, 12);
    Console.WriteLine("Värvid:");
    Console.SetCursorPosition(50, 13);
    ColorConsole.WriteLine("~--Ta(~-4u~--)st:\t" + this._colorScheme.BackgroundToHexString());
    Console.SetCursorPosition(50, 14);
    ColorConsole.WriteLine("~--(~-2E~--)siplaan:\t" + this._colorScheme.ForegroundToHexString());
  }
}