using MasFlashDrv.Config.Drives;
using MasTUICommon.Components;

namespace MasFlashDrv.Tabs
{
    internal class Development : TabBase
    {
        private Edition CurrentDrive { get; set; }

        private string[] Videos { get; set; }

        private int SelectedIndex {  get; set; }

        internal Development(Edition currentDrive)
        {
            CurrentDrive = currentDrive;
            TextReader tr = File.OpenText(Path.Join(CurrentDrive.Mount, "E_INFO", "videod.txt"));
            var vData = tr.ReadToEnd();
            tr.Close();
            tr.Dispose();
            Videos = vData.Replace("\r\n", "\n").Replace("\n", "").Split(';');
        }

        public override void Draw(object sender, EventArgs e)
        {
            Console.WriteLine("Esiletõstetud videote haldamine");
            Console.WriteLine();
            foreach (var (i, v) in Videos.Index())
            {
                ColorConsole.WriteLine((i == SelectedIndex ? "~-F > " : "~-8   ") + v);
            }
            Console.WriteLine();
            ColorConsole.WriteLine("~-- (~-EL~--)isa video");
            ColorConsole.WriteLine("~-- (~-9A~--)senda video");
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            FilePicker fp;
            if (Program.C?.Status == "VERIFIED")
            {
                fp = new(Config.Integration.DecodeScheme()) { Directory = CurrentDrive.Mount };
            }
            else
            {
                fp = new() { Directory = CurrentDrive.Mount };
            }
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    if (SelectedIndex < 2) SelectedIndex++;
                    break;
                case ConsoleKey.UpArrow:
                    if (SelectedIndex > 0) SelectedIndex--;
                    break;
                case ConsoleKey.L:                    
                    fp.FileOk += AddVideoFp_FileOk;
                    fp.Show();
                    Console.Clear();
                    MainScreen.DrawTitleBar();
                    break;
                case ConsoleKey.A:
                    fp.FileOk += ReplaceVideo_FileOk;
                    fp.Show();
                    Console.Clear();
                    MainScreen.DrawTitleBar();
                    break;
            }
        }

        private void ApplyChanges()
        {
            var tw = File.CreateText(Path.Join(CurrentDrive.Mount, "E_INFO", "videod.txt"));
            tw.WriteLine(string.Join(';', Videos));
            tw.Close();
            tw.Dispose();
        }

        private void ReplaceVideo_FileOk(object sender, FilePickerEventArgs e)
        {
            if (e.FileName == null) return;
            File.Move(e.FileName, Path.Join(CurrentDrive.Mount, "Markuse_videod", $"{SelectedIndex+1}. {new FileInfo(e.FileName).Name}"));
            File.Move(Path.Join(CurrentDrive.Mount, "Markuse_videod", $"{SelectedIndex+1}. {Videos[SelectedIndex]}"), Path.Join(new FileInfo(e.FileName).Directory?.FullName ?? CurrentDrive.Mount, Videos[SelectedIndex]));
            Videos[SelectedIndex] = new FileInfo(e.FileName).Name;
            ApplyChanges();
        }

        private void AddVideoFp_FileOk(object sender, FilePickerEventArgs e)
        {
            if (e.FileName == null) return;
            File.Move(e.FileName, Path.Join(CurrentDrive.Mount, "Markuse_videod", "1. " + new FileInfo(e.FileName).Name));
            File.Move(Path.Join(CurrentDrive.Mount, "Markuse_videod", "1. " + Videos[0]), Path.Join(CurrentDrive.Mount, "Markuse_videod", "2. " + Videos[0]));
            File.Move(Path.Join(CurrentDrive.Mount, "Markuse_videod", "2. " + Videos[1]), Path.Join(CurrentDrive.Mount, "Markuse_videod", "3. " + Videos[1]));
            File.Move(Path.Join(CurrentDrive.Mount, "Markuse_videod", "3. " + Videos[2]), Path.Join(new FileInfo(e.FileName).Directory?.FullName, Videos[2]));
            var oldVideos = new string[3];
            Array.Copy(Videos, oldVideos, 3);
            Videos[0] = new FileInfo(e.FileName).Name;
            Videos[1] = oldVideos[0];
            Videos[2] = oldVideos[1];
            ApplyChanges();
        }
    }
}
