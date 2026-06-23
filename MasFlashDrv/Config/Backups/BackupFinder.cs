namespace MasFlashDrv.Config.Backups
{
    internal class BackupFinder
    {
        public string? RootDirectory { get; set; }

        public List<Backup> Backups { get; set; }

        public BackupFinder()
        {
            foreach (var di in DriveInfo.GetDrives())
            {
                if (Directory.Exists(Path.Join(di.RootDirectory.FullName, "Varukoopiad")))
                {
                    RootDirectory = Path.Join(di.RootDirectory.FullName, "Varukoopiad");
                    break;
                }
            }
            Backups = [];
            if (RootDirectory == null) { return; }
            foreach (var dir in new DirectoryInfo(RootDirectory).EnumerateDirectories())
            {
                if (File.Exists(Path.Join(dir.FullName, ".MISSING")))
                {
                    TextReader tr = File.OpenText(Path.Join(dir.FullName, ".MISSING"));
                    List<string> depends = [];
                    foreach (var l in tr.ReadToEnd().Replace("\r\n", "\n").Split('\n'))
                    {
                        depends.Add(l);
                    }
                    Backups.Add(new() { MissingFiles = [.. depends], Location = dir.FullName });
                    Backups[^1].Init();
                }
            }
        }
    }
}
