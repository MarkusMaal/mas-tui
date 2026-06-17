using System.Drawing;
using ConsoleImage.Core;
using MasCommon;
using MasCpanel.Config;
using MasTUICommon.Components;
using Color = MasTUICommon.Color;

namespace MasCpanel.Tabs;

public class Configuration : TabBase
{

  private string? _desktopPreview;
  private string? _uncommonPreview;
  private string? _loginPreview;
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
    ReloadPreviews();
    Program.L.StatusText = "Loading integration configuration";
    _config = new CommonConfig();
    _config.Load(_masRoot);
    _logoCheck.Value = _config.ShowLogo;
    _desktopCheck.Value = _config.AutostartNotes;
    _scheduleCheck.Value = _config.AllowScheduledTasks;
    Program.L.StatusText = "Loading color scheme";
    _colorScheme.LoadScheme(_masRoot);
  }

  private void ReloadPreviews()
  {
    using var colorBlockRenderer = new ColorBlockRenderer(new RenderOptions
    {
      MaxHeight = 5
    });
    _desktopPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_desktop.png"));
    _uncommonPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_uncommon.png"));
    _loginPreview = colorBlockRenderer.RenderFile(Path.Join(_masRoot, "bg_login.png"));
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
      case ConsoleKey.D1:
      case ConsoleKey.NumPad1:
        ShowFilePicker(sender,
          (_, e) => CopyBackground(e.FileName, "bg_desktop.png"),
          (_, _) => {});
        break;
      case ConsoleKey.D2:
      case ConsoleKey.NumPad2:
        ShowFilePicker(sender,
          (_, e) => CopyBackground(e.FileName, "bg_login.png"),
          (_, _) => {});
        break;
      case ConsoleKey.D3:
      case ConsoleKey.NumPad3:
        ShowFilePicker(sender, 
          (_, e) => CopyBackground(e.FileName, "bg_uncommon.png"), 
          (_, _) => {});
        break;
      case ConsoleKey.V:
        SwitchBackground(sender);
        break;
    }
    Console.WriteLine("\r   ");
  }

  private void SwitchBackground(object sender)
  {
    if (sender is not MainScreen ms) return;
    File.Copy( Path.Join(_masRoot, "bg_desktop.png"), Path.Join(_masRoot, "bg_temp.png"), true);
    File.Copy( Path.Join(_masRoot, "bg_uncommon.png"), Path.Join(_masRoot, "bg_desktop.png"), true);
    File.Copy( Path.Join(_masRoot, "bg_temp.png"), Path.Join(_masRoot, "bg_uncommon.png"), true);
    File.Delete(Path.Join(_masRoot, "bg_temp.png"));
    ReloadPreviews();
    ms.Cls();
  }

  private void CopyBackground(string? source, string destination)
  {
    if (source == null) return;
    if (!new FileInfo(source).Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) return;
    File.Copy(source,  Path.Join(_masRoot, destination), true);
    ReloadPreviews();
  }

  private void ShowFilePicker(object sender, FilePicker.FileOkHandler okHandle, FilePicker.FileCancelHandler cancelHandle)
  {
    var fp = new FilePicker();
    fp.FileOk += okHandle;
    fp.FileCancel += cancelHandle;
    fp.FileChange += (sender, e) =>
    {
      if (Console.WindowWidth < 120) return;
      if (sender is not FilePicker fp) return;
      var fsI = new FileInfo(e.FileName ?? "");
      var maxWide = new DirectoryInfo(fp.Directory).GetFileSystemInfos()
        .OrderBy(fI => fI is not DirectoryInfo)
        .ThenBy(fI => fI.Name)
        .Where(fI => (fI.Attributes & FileAttributes.Hidden) == 0).Max(fI => fI.Name.Length + (fI is DirectoryInfo ? 1 : 0)) + 6;
      for (var i = 0; i < Console.WindowHeight - 4; i++)
      {
        Console.SetCursorPosition(maxWide, 3 + i);
        Console.WriteLine("".PadRight(Console.WindowWidth - 1 - maxWide));
      }
      if (!fsI.Exists || !fsI.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }
      using var colorBlockRenderer = new ColorBlockRenderer(new RenderOptions
      {
        MaxHeight = Console.WindowHeight - 4,
        MaxWidth = Console.WindowWidth - 56
      });
      try
      {
        var preview = colorBlockRenderer.RenderFile(e.FileName ?? "");
        ShowAnsiAtPosition(preview, maxWide + 1, 3);
      }
      catch
      {
        // ignored
      }
    };
    fp.Show();
    if (sender is MainScreen ms)
    {
      ms.Cls();
    }
  }

  private (int, int) GetAnsiDims(string ansi)
  {
    return (ansi.Split('\n')[0].Split("\e[0m").Length, ansi.Split('\n').Length);
  }

  private void ShowAnsiAtPosition(string ansi, int x, int y)
  {
    foreach (var (index, item) in ansi.Split('\n').Index())
    {
      Console.SetCursorPosition(x, y + index);
      Console.Write(item);
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
    var (num1, _) = GetAnsiDims(_desktopPreview);
    var (num2, _) = GetAnsiDims(_loginPreview);
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