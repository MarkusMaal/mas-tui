using MasFlashDrv.Config.Drives;

namespace MasFlashDrv.Config.Stats
{
    internal class SCounter(Edition drive)
    {
        // all sizes in bytes (B)

        public long TotalUsed { get; init; } = GetSize(new DirectoryInfo(drive.Mount));

        public long MarkusStuff { get; init; } = GetSize(new DirectoryInfo(Path.Join(drive.Mount, "markuse asjad", "markuse asjad"))) + GetSize(new DirectoryInfo(Path.Join(drive.Mount, "Markuse_videod")));

        public long QuickApps { get; init; } = GetSize(new DirectoryInfo(Path.Join(drive.Mount, "markuse asjad", "Kiirrakendused")));

        public long OperatingSystems { get; init; } = GetSize(new DirectoryInfo(Path.Join(drive.Mount, "multiboot")));

        public long BatchFiles { get; init; } = GetSize(new DirectoryInfo(Path.Join(drive.Mount, "Pakkfailid")));

        public long Other => TotalUsed - MarkusStuff - QuickApps - BatchFiles - OperatingSystems;

        public long FreeSpace => drive.FsInfo.TotalFreeSpace;


        // find the directory size with linq recursion
        // note that we also try to avoid branching to junctions
        private static long GetSize(DirectoryInfo searchDir)
        {
            if (DateTime.Now.Ticks % 500 == 0) Program.L.StatusText = "Statistika kogumine";
            if (!Directory.Exists(searchDir.FullName)) return 0;
            return searchDir.EnumerateFiles().Sum(p => p.Length) + 
                searchDir.EnumerateDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.ReparsePoint) && (d.Name != "System Volume Information")).Sum(GetSize);
        }

        // for printing the size in a user friendly format
        public static string FriendlySize(long bytes, bool binary = false, int rounding = 2)
        {
            var unit = binary ? 1024 : 1000;  // storage is advertised with decimal units, while OSes like to use binary units
            var suffix = binary ? "iB" : "B"; // this method allows you to format the size in a friendly way using both methods

            if (bytes < unit) return $"{bytes}B"; // less than unit size
            else if (bytes < Math.Pow(unit, 2)) return $"{Math.Round(bytes / (double)unit, rounding)} k{suffix}";      // kilo/kibi \
            else if (bytes < Math.Pow(unit, 3)) return $"{Math.Round(bytes / Math.Pow(unit, 2), rounding)} M{suffix}"; // mega/mebi  |
            else if (bytes < Math.Pow(unit, 4)) return $"{Math.Round(bytes / Math.Pow(unit, 3), rounding)} G{suffix}"; // giga/gibi  |  byte(s)
            else if (bytes < Math.Pow(unit, 5)) return $"{Math.Round(bytes / Math.Pow(unit, 4), rounding)} T{suffix}"; // tera/tebi  |
            else if (bytes < Math.Pow(unit, 6)) return $"{Math.Round(bytes / Math.Pow(unit, 5), rounding)} P{suffix}"; // peta/pebi  |
            else return $"{Math.Round(bytes / Math.Pow(unit, 6), rounding)} E{suffix}";                                // exa/exbi  /
        }
    }
}
