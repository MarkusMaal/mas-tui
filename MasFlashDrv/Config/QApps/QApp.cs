using System.Diagnostics;

namespace MasFlashDrv.Config.QApps
{
    internal class QApp(string driveRoot)
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required byte[] Screenshot { get; set; }

        private string DriveRoot { get; init; } = driveRoot;

        public string? Executable
        {
            get
            {
                var rootBase = Path.Join(DriveRoot, "markuse asjad", "Kiirrakendused", Name);
                if (OperatingSystem.IsWindows()) {
                    return Path.Join(rootBase, Name + "Portable.exe");
                }
                else if (OperatingSystem.IsLinux())
                {
                    return Path.Join(rootBase, Name + "Portable.AppImage");
                }
                else if (OperatingSystem.IsMacOS())
                {
                    return Path.Join(rootBase, Name + "Portable.app");
                } else
                {
                    return null;
                }
            }
        }

        public void Run()
        {
            ProcessStartInfo? si = null;
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            {
                si = new ProcessStartInfo()
                {
                    FileName = Executable,
                    UseShellExecute = true
                };
            } else if (OperatingSystem.IsMacOS())
            {
                si = new ProcessStartInfo()
                {
                    FileName = "open",
                    Arguments = $"-a {Executable}",
                    UseShellExecute = true
                };
            }
            if (si == null) throw new PlatformNotSupportedException();
            Process p = new()
            {
                StartInfo = si
            };
            p.Start();
        }
    }
}
