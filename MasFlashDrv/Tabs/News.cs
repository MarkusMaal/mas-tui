using MasTUICommon;
using MasTUICommon.Components;
using System.Diagnostics;

namespace MasFlashDrv.Tabs
{
    internal class News : TabBase
    {
        private int SelectedArticle
        {
            get; set
            {
                Skip = 0;
                if (value > Program.F.Articles.Count - 1) return;
                field = value;
            }
        }

        private int SelectedVideo
        {
            get; set;
        } = 0;

        private int Skip { get; set; }

        private string[] Videos { get; set; }

        private Config.Drives.Edition CurrentDrive { get; init; }

        private bool SkipArticleDraw { get; set; } = false;

        private int VidContainerWidth => Videos.Max(v => v.Length) + 12;

        public News(Config.Drives.Edition currentDrive)
        {
            Program.L.StatusText = "Uusimate videote avastamine";
            CurrentDrive = currentDrive;
            var videoPath = Path.Join(currentDrive.Mount, "E_INFO", "videod.txt");
            TextReader tr = File.OpenText(videoPath);
            Videos = tr.ReadToEnd().Split(';');
            tr.Close();
            tr.Dispose();
            var tempFeedFile = Path.Join(Path.GetTempPath(), "mas_flashdrv_feed.xml");
            var flashNewsFile = Path.Join(currentDrive.Mount, "E_INFO", "news.xml");
            if (File.Exists(tempFeedFile))
            {
                File.Copy(tempFeedFile, flashNewsFile, true);
                File.Delete(tempFeedFile);
            }
            if (Program.F.Articles.Count == 0 && File.Exists(flashNewsFile))
            {
                Program.F.Read(flashNewsFile);
            }
        }

        public override void Draw(object sender, EventArgs e)
        {
            var maxWidth = Console.WindowWidth - VidContainerWidth;
            var hideVidContainer = false;
            if (maxWidth < 20)
            {
                maxWidth = Console.WindowWidth - 4;
                hideVidContainer = true;
            }
            if (!SkipArticleDraw) ClearArticle();
            var titleMaxWidth = maxWidth + 2 - Program.F.Articles.Count * 2;
            Console.SetCursorPosition(titleMaxWidth, 3);
            for (var i = 0; i < Program.F.Articles.Count; i++)
            {
                ColorConsole.Write((SelectedArticle == i ? "~-F" : "~-8") + $"{i + 1}~-- ");
            }
            if (!hideVidContainer)
            {
                Console.SetCursorPosition(maxWidth + 5, 3);
                Console.Write("Uusimad videod");
                foreach (var (i, v) in Videos.Index())
                {
                    if (i > 4) break;
                    Console.SetCursorPosition(maxWidth + 5, 5 + i);
                    var c = SelectedVideo == i ? "~-F" : "~-8";
                    ColorConsole.Write($"{c}{i + 1}. {v}~--");
                }
            }
            Console.SetCursorPosition(0, 3);
            if (SkipArticleDraw)
            {
                SkipArticleDraw = false;
                return;
            }
            var selectedArticleStr = Program.F.Articles[SelectedArticle].ToString();
            var title = selectedArticleStr.Split('\n')[0];
            if (title.Length > titleMaxWidth) title = title[..titleMaxWidth];
            ColorConsole.WriteLine("~-F" + title + "~--");
            ColorConsole.WriteLine("~--" + selectedArticleStr.Split('\n')[1]);
            Console.CursorTop++;
            var fixedArticle = Utils.WrapLines(string.Join('\n', selectedArticleStr.Split('\n').Skip(2).ToArray()), maxWidth);
            foreach (var c in fixedArticle.Split('\n').Skip(1 + Skip))
            {
                if (Console.CursorLeft > maxWidth)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop++;
                }
                if ((Console.CursorTop < Console.WindowHeight - 2) ||
                    (Console.CursorTop == Console.WindowHeight - 2 && Console.CursorLeft < Console.WindowWidth - 1))
                {
                    Console.WriteLine(c);
                }
            }
        }

        private void ClearArticle()
        {
            Console.SetCursorPosition(0, 3);
            Console.Write("".PadRight(Console.WindowWidth * (Console.WindowHeight - 4)));
        }

        public override void ReceiveKey(object sender, ConsoleKey key)
        {
            var maxWidth = Console.WindowWidth - VidContainerWidth;
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    SelectedVideo++;
                    if (SelectedVideo > Videos.Length - 1) SelectedVideo--;
                    SkipArticleDraw = true;
                    break;
                case ConsoleKey.UpArrow:
                    SelectedVideo--;
                    if (SelectedVideo < 0) SelectedVideo++;
                    SkipArticleDraw = true;
                    break;
                case ConsoleKey.S:
                    Skip++;
                    if (Skip > Program.F.Articles[SelectedArticle].GetLastSkip(maxWidth)) Skip--;
                    break;
                case ConsoleKey.W:
                    Skip--;
                    if (Skip < 0) Skip = 0;
                    break;
                case ConsoleKey.PageDown:
                    Skip += Console.WindowHeight - 4;
                    if (Skip > Program.F.Articles[SelectedArticle].GetLastSkip(maxWidth)) Skip = Program.F.Articles[SelectedArticle].GetLastSkip(maxWidth);
                    break;
                case ConsoleKey.PageUp:
                    Skip -= Console.WindowHeight - 4;
                    if (Skip < 0) Skip = 0;
                    break;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    SelectedArticle = 0;
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    SelectedArticle = 1;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    SelectedArticle = 2;
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    SelectedArticle = 3;
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    SelectedArticle = 4;
                    break;
                case ConsoleKey.Enter:
                    Process p = new()
                    {
                        StartInfo = {
                            UseShellExecute = true,
                            FileName = Path.Join(CurrentDrive.Mount, "Markuse_videod", $"{SelectedVideo+1}. {Videos[SelectedVideo].Replace("\n", "")}"),
                        }
                    };
                    p.Start();
                    break;
            }
        }
    }
}
