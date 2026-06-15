using MasCommon;
using MasTUICommon;

namespace MasCpanel
{
  internal abstract class Program
  {
    public static readonly Loader L = new();

    public static readonly Config.MarkuStation.Config MsConfig = new();

    private static int Main()
    {
      L.StatusTextChanged += LoadCheck;
      new Thread(SpinLoader).Start();
      var status = "";
      
      var task = new Task(() =>
      {
        L.StatusText = "Checking for Verifile tamper";
        var vf = new Verifile();
        if (!Verifile.CheckVerifileTamper())
        {
          status = "FAILED";
          return;
        }
        L.StatusText = "Checking for Verifile status";
        status = vf.MakeAttestation();
      });

      task.Start();
      task.Wait();
      if (status != "VERIFIED")
      {
        Console.WriteLine("The computer did not pass Verifile attestation. Closing the program now...");
        return 1;
      }

      L.StatusText = "Interpreting MarkuStation data";
      MsConfig.LoadConfig();
      
      new MainScreen
      {
        VerifileStatus = status,
      }.Show();
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

