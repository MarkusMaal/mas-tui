using System.Diagnostics;
using MasCommon;
using MasTUICommon;
using Color = System.Drawing.Color;

namespace MasCpanel
{
  internal abstract class Program
  {
    public static readonly Loader L = new();
    public static readonly Config.MarkuStation.Config MsConfig = new();
    
    public static Color Background = Color.DimGray;
    public static Color Foreground = Color.White;

    public static void Reload()
    {
      Main();
    }
    
    private static int Main()
    {
      Console.CancelKeyPress += (_, e) =>
      {
        if (e.SpecialKey != ConsoleSpecialKey.ControlC) return;
        if (e.SpecialKey == ConsoleSpecialKey.ControlBreak) Process.GetCurrentProcess().Kill();
        Console.Clear();
        Console.Error.WriteLine("Rakenduse ohutu peatamine nurjus");
      };
      L.StatusTextChanged += LoadCheck;
      new Thread(SpinLoader).Start();
      var status = "";
    
      var task = new Task(() =>
      {
        L.StatusText = "Verifile võltsingu kontroll";
        var vf = new Verifile();
        if (!Verifile.CheckVerifileTamper())
        {
          status = "FAILED";
          return;
        }
        L.StatusText = "Verifile oleku kontroll";
        status = vf.MakeAttestation();
      });

      task.Start();
      task.Wait();
      if (status != "VERIFIED")
      {
        Console.WriteLine("Verifile kontroll ebaõnnestus. Programmi sulgemine...");
        Environment.Exit(1);
      }
      MsConfig.LoadConfig();
      var mainScreen = new MainScreen
      {
        VerifileStatus = status,
      };
      mainScreen.Show();
      Console.Clear();
      return 0;
    }

    public static void SpinLoader()
    {
      Console.CursorVisible = false;
      while (true)
      {
        L.StatusText = L.StatusText;
        Thread.Sleep(1);
        if (L.StatusText == "") break;
      }
      Console.CursorVisible = true;
    }

    private static void LoadCheck(string status)
    {
      if (status != "")
      {
        L.PrintLoader();
      }
      else
      {
        Console.Write("".PadRight(Console.WindowWidth - 1) + "\r");
      }
    }
  }
};

