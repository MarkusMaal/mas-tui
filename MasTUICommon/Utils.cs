using MasTUICommon.Components;
using System.Diagnostics;
using System.Text;

namespace MasTUICommon
{
    public abstract class Utils
    {
        public static string WrapLines(string content, int constraint)
        {
            StringBuilder sb = new();
            foreach (var l in content.Split('\n'))
            {
                var debt = 0;
                var words = new List<string>();
                foreach (var w in l.Split(' '))
                {
                    if (debt + w.Length > constraint)
                    {
                        debt = 0;
                        sb.AppendLine(string.Join(' ', words));
                        words.Clear();
                    }
                    words.Add(w);
                    debt += w.Length + 1;
                }
                sb.AppendLine(string.Join(' ', words));
                debt = 0;
            }
            return sb.ToString();
        }

        public static void CheckCodepage()
        {
            if (OperatingSystem.IsWindows())
            {
                // a hack to force Windows command host window to use UTF-8 instead of OEM-XYZ encoding
                var p = new Process()
                {
                    StartInfo = {
                        FileName = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.System), "chcp.com"), // C:\Windows\System32\chcp.com (in most cases)
                        Arguments = "65001", // = UTF-8
                        UseShellExecute = false, // pretty please use the current window
                        RedirectStandardOutput = true,
                    }
                };
                p.Start();
                p.WaitForExit();
            }
            if (Console.OutputEncoding.CodePage != 65001)
            {
                ColorConsole.WriteLine("~-EHoiatus~--: Käsuaken ei kasuta UTF-8 väljundi kodeeringut, mõned märgid võivad ilmuda valesti!");
            }
        }



        public static ConsoleKeyInfo? TimeoutReadKey(int timeout)
        {
            var task = Task.Run(() => Console.ReadKey(true));
            bool read = task.Wait(timeout);
            if (read) return task.Result;
            return null;
        }

    }
}
