using System.Globalization;

namespace MasFlashDrv.Config.Backups
{
    internal class Backup
    {
        public required string Location { get; set; }
        public required string[] MissingFiles { get; set; }
        public string Name => new DirectoryInfo(Location).Name.Split(" (")[0];
        private DateTime Creation { get; set; }
        private string newLine = OperatingSystem.IsWindows() ? "\r\n" : "\n";


        public void Init()
        {
            string date_elements = new DirectoryInfo(Location).Name.Split('(')[1].Replace(")", "");
            string[] datums = date_elements.Split('-');
            Creation = new DateTime(int.Parse(datums[0]), int.Parse(datums[1]), int.Parse(datums[2]));
        }

        public override string ToString()
        {
            return Name + " - " + Creation.Date.ToString("d. MMMM yyyy", new CultureInfo("et-EE"));
        }

        public string[] AllFiles => [.. (string.Join(newLine, MissingFiles) + RecurseFiles(Location)).Split(newLine).Where(p => p != "")];

        public void Rename(string newName)
        {
            var parent = new DirectoryInfo(Location).Parent;
            if (parent == null) return;
            Directory.Move(Location, Path.Join(parent.FullName, $"{newName} ({Creation.Year}-{Creation.Month}-{Creation.Day})"));
        }

        public void Delete()
        {
            Directory.Delete(Location, true);
        }

        private string RecurseFiles(string root)
        {
            var o = "";
            o += string.Join(newLine, new DirectoryInfo(root).EnumerateFiles().Where(p => p.FullName != Path.Join(Location, ".MISSING")).Select(p => p.FullName.Replace(Location, "")));
            foreach (var d in new DirectoryInfo(root).EnumerateDirectories().Where(p => !p.Attributes.HasFlag(FileAttributes.ReparsePoint)))
            {
                o += RecurseFiles(d.FullName);
            }
            while (o.Contains(newLine + newLine))
            {
                o = o.Replace(newLine + newLine, newLine);
            }
            return o + newLine;
        }
    }
}
