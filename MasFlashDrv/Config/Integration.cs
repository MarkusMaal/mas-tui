using MasCommon;

namespace MasFlashDrv.Config
{
    internal class Integration
    {
        public string Status { get; set; }

        private static string MasRoot => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        private bool Init { get; set; } = true;

        public bool DarkMode { get; set {
                WriteText(Path.Join(MasRoot, "settings.sf"), "NightMode=" + (value ? "Yes" : "No") + ";");
                field = value;
        }} = false;

        public bool AutoRun { get; set
        {
                WriteText(Path.Join(MasRoot, "settings2.sf"), "AutoRun=" + (value ? "true" : "false"));
                field = value;
        }} = false;

        public Integration()
        {
            Program.L.StatusText = "Verifile võltsingu kontroll";
            var isTampered = !Verifile.CheckVerifileTamper();
            if (isTampered)
            {
                Status = "FAILED";
                return;
            }
            Verifile vf = new();
            Program.L.StatusText = "Verifile oleku kontroll";
            Status = vf.MakeAttestation();
            if (Status != "VERIFIED") return;
            Program.L.StatusText = "Seadete laadimine";
            var darkModeSf = Path.Join(MasRoot, "settings.sf");
            var autoRunSf = Path.Join(MasRoot, "settings2.sf");
            TextReader tr;
            Init = false;
            if (File.Exists(darkModeSf))
            {
                tr = File.OpenText(darkModeSf);
                Init = true;
                DarkMode = tr.ReadLine()?.Contains("NightMode=Yes") ?? false;
                Init = false;
                tr.Close();
                tr.Dispose();
            }
            if (!File.Exists(autoRunSf)) return;
            tr = File.OpenText(autoRunSf);
            Init = true;
            AutoRun = tr.ReadLine()?.Contains("AutoRun=true") ?? false;
            Init = false;
            tr.Close();
            tr.Dispose();
        }

        private void WriteText(string destination, string content)
        {
            if (Init) return;
            TextWriter tw = File.CreateText(destination);
            tw.Write(content);
            tw.Close();
            tw.Dispose();
        }
    }
}
