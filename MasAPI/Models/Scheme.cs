namespace MasAPI.Models
{

    public class Scheme
    {
        public Types.Color? BackgroundColor { get; set; }

        public Types.Color? ForegroundColor { get; set; }

        public void LoadScheme(string masRoot)
        {
            TextReader textReader = File.OpenText(Path.Join(masRoot, "scheme.cfg"));
            var strArray1 = textReader.ReadLine()?.Split(';');
            if (strArray1 == null)
                return;
            var strArray2 = strArray1[0].Split(':');
            var strArray3 = strArray1[1].Split(':');
            BackgroundColor = new Types.Color()
            {
                A = 255,
                R = int.Parse(strArray2[0]),
                G = int.Parse(strArray2[1]),
                B = int.Parse(strArray2[2])
            };
            ForegroundColor = new Types.Color()
            {
                A = 255,
                R = int.Parse(strArray3[0]),
                G = int.Parse(strArray3[1]),
                B = int.Parse(strArray3[2])
            };
            textReader.Close();
            textReader.Dispose();
        }

        public void SaveScheme(string masRoot)
        {
            TextWriter text = File.CreateText(Path.Join(masRoot, "scheme.cfg"));
            text.Write($"{BackgroundColor?.R}:{BackgroundColor?.G}:{BackgroundColor?.B}:;{ForegroundColor?.R}:{ForegroundColor?.G}:{ForegroundColor?.B}:;");
            text.Close();
            text.Dispose();
        }

        public string BackgroundToHexString() => ColorToHexString(BackgroundColor);

        public string ForegroundToHexString() => ColorToHexString(ForegroundColor);

        public string BackgroundToAnsi() => ColorToAnsi(BackgroundColor);

        public string ForegroundToAnsi() => ColorToAnsi(ForegroundColor);

        private static string ColorToHexString(Types.Color? color)
        {
            if (color == null) return "#000000";
            var num = color.R;
            var str1 = num.ToString("X").PadLeft(2, '0');
            num = color.G;
            var str2 = num.ToString("X").PadLeft(2, '0');
            num = color.B;
            var str3 = num.ToString("X").PadLeft(2, '0');
            return $"#{str1}{str2}{str3}";
        }

        private static string ColorToAnsi(Types.Color? color)
        {
            return
                $"\e[48;2;{color?.R};{color?.G};{color?.B}m";
        }

    }
}
