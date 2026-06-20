using MasFlashDrv.Config.Drives;
using System.Diagnostics;

namespace MasFlashDrv.Config.Dirs
{
    internal class Users
    {
        public DirectoryInfo[] UserDirs { get; set; }

        internal Users(Edition currentDrive)
        {
            UserDirs = [.. new DirectoryInfo(Path.Join(currentDrive.Mount, "markuse asjad", "markuse asjad")).EnumerateDirectories().Where(p => p.Name != "Mine")];
        }

        public DirectoryInfo[] GetSubdirs(string username)
        {
            return [.. UserDirs.First(p => p.Name == username).EnumerateDirectories()];
        }

        public static void OpenDir(DirectoryInfo di)
        {
            ProcessStartInfo? si = null;
            if (OperatingSystem.IsWindows())
            {
                si = new()
                {
                    FileName = "explorer",
                    Arguments = "\"" + di.FullName + "\""
                };
            } else if (OperatingSystem.IsLinux())
            {
                si = new()
                {
                    FileName = "xdg-open",
                    Arguments = $"\"{di.FullName}\""
                };
            } else if (OperatingSystem.IsMacOS())
            {
                si = new()
                {
                    FileName = "open",
                    Arguments = $"-a Finder.app \"{di.FullName}\""
                };
            }
            if (si == null) throw new PlatformNotSupportedException();
            si.UseShellExecute = true;
            new Process()
            {
                StartInfo = si
            }.Start();
        }
    }
}
