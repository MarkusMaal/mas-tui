using MasFlashDrv.Config.Drives;
using MasFlashDrv.Config.QApps;
using MasTUICommon.Components;
using System.Diagnostics;

namespace MasFlashDrv.Tabs
{
    internal class QuickApps : TabBase
    {

        private QAppFinder QAppFinder { get; set; }
        private Menu AppMenu { get; set; }

        internal QuickApps(Edition drive)
        {
            QAppFinder = new QAppFinder(drive.Mount);
            AppMenu = new Menu()
            {
                ActiveColor = new MasTUICommon.Color
                {
                    BackgroundColor = 0x7,
                    ForegroundColor = 0x0
                },
                DefaultColor = new MasTUICommon.Color
                {
                    BackgroundColor = 0x10,
                    ForegroundColor = 0x7,
                },
                MarginLeft = 1,
                MarginTop = 3,
                TextPadding = QAppFinder.Apps.Max(p => p.Name.Length) + 6
            };

            foreach (var app in QAppFinder.Apps)
            {
                AppMenu.AddItem(app.Name, (_, _) => { app.Run(); }, app.Description);
            }

        }

        public override void Draw(object sender, EventArgs e)
        {
            AppMenu.Draw();
            var thumbMsg = "Kuva (p)isipilt";
            Console.SetCursorPosition(Console.WindowWidth - thumbMsg.Length - 3, 3);
            ColorConsole.Write($"~--{thumbMsg.Replace("(", "(~-A").Replace(")", "~--)")}");
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (AppMenu.SelectedIndex > 0) AppMenu.SelectedIndex--;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (AppMenu.SelectedIndex < QAppFinder.Apps.Count - 1) AppMenu.SelectedIndex++;
                    break;
                case ConsoleKey.Enter:
                    AppMenu.Execute();
                    break;
                case ConsoleKey.P:
                    var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".bmp");
                    File.WriteAllBytes(tempFile, this.QAppFinder.Apps[AppMenu.SelectedIndex].Screenshot);
                    var p = new Process()
                    {
                        StartInfo =
                        {
                            FileName = tempFile,
                            UseShellExecute = true
                        }
                    };
                    p.Start();
                    p.WaitForExit();
                    File.Delete(tempFile);
                    break;
            }
        }
    }
}
