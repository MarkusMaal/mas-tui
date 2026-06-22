using System.Security.Cryptography;
using System.Text;

namespace MasFlashDrv.Config.Drives
{
    internal class Edition
    {
        public string Mount { get; set; }

        public string EditionName { get; set; }

        public string? Pin { get; set; }

        public bool IsLegacyPinEnabled => LegacyPin != "Ebaturvaline PIN kood keelatud";

        public string? LegacyPin { get; set; }

        public DriveInfo FsInfo { get; set; }

        public bool Unlocked { get; set; }

        public static bool IsDriveCompatible(string path)
        {
            return File.Exists(Path.Join(path, "E_INFO", "edition.txt")) && File.Exists(Path.Join(path, "NTFS", "config.sys"));
        }

        public Edition(string mount)
        {
            Mount = mount;
            FsInfo = new DriveInfo(mount);
            TextReader tr = File.OpenText(Path.Join(mount, "E_INFO", "edition.txt"));
            EditionName = tr.ReadToEnd();
            tr.Close();
            tr.Dispose();
            ReloadPins();
        }

        public void ReloadPins()
        {
            TextReader tr = File.OpenText(Path.Join(Mount, "NTFS", "config.sys"));
            LegacyPin = tr.ReadLine() ?? "";
            tr.Close();
            tr.Dispose();
            var spinFile = Path.Join(Mount, "NTFS", "spin.sys");
            if (File.Exists(spinFile))
            {
                tr = File.OpenText(spinFile);
                Pin = tr.ReadLine() ?? "";
                tr.Close();
                tr.Dispose();
            }
            Unlocked = false;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Asukoht: {Mount}");
            sb.AppendLine($"PIN (ebaturvaline): {LegacyPin}");
            sb.AppendLine($"Väljaanne: {EditionName}");
            sb.AppendLine();
            sb.AppendLine($"Silt: {FsInfo.VolumeLabel}");
            sb.AppendLine($"Failisüsteem: {FsInfo.DriveFormat}");
            sb.AppendLine($"Maht: {Math.Round(FsInfo.TotalSize / 1000.0 / 1000.0 / 1000.0)} GB");
            return sb.ToString();
        }

        public bool CheckPin(string providedPin)
        {
            if (Pin != null)
            {
                return CreateMD5(providedPin) == Pin;
            } else
            {
                return providedPin == LegacyPin;
            }
        }

        public static string GenerateSecurePin(string providedPin)
        {
            return CreateMD5(providedPin);
        }

        private static string CreateMD5(string input)
        {
            var o = "";
            foreach (var b in MD5.HashData(Encoding.ASCII.GetBytes(input)))
            {
                o += b.ToString("X2");
            }
            return o;
        }
    }
}