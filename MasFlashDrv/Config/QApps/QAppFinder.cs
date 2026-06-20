namespace MasFlashDrv.Config.QApps
{
    internal class QAppFinder
    {
        public List<QApp> Apps { get; set; } = [];

        public QAppFinder(string driveRoot)
        {
            Program.L.StatusText = "Kiirrakenduste otsimine";
            var appDirs = new DirectoryInfo(Path.Join(driveRoot, "markuse asjad", "Kiirrakendused"))
                .EnumerateDirectories()
                .Where(p => File.Exists(Path.Join(p.FullName, p.Name + "Portable.exe")) ||
                            File.Exists(Path.Join(p.FullName, p.Name + "Portable.AppImage")));
            foreach (var appDir in appDirs)
            {
                var infoTxt = Path.Join(appDir.FullName, appDir.Name + "Info.txt");
                var screenShot = Path.Join(appDir.FullName, appDir.Name + "ScreenShot.bmp");
                Apps.Add(new QApp(driveRoot)
                {
                    Name = appDir.Name,
                    Description = File.Exists(infoTxt) ? File.ReadAllText(infoTxt, System.Text.Encoding.Unicode) : "(kirjeldus puudub)",
                    Screenshot = File.Exists(screenShot) ? File.ReadAllBytes(screenShot) : []
                });
            }
        }
    }
}
