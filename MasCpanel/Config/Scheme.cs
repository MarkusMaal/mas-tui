using System.Drawing;

namespace MasCpanel.Config;

public class Scheme
{
    public Color BackgroundColor { get; set; }

    public Color ForegroundColor { get; set; }

    public void LoadScheme(string masRoot)
    {
        TextReader textReader = (TextReader) File.OpenText(Path.Join(masRoot, "scheme.cfg"));
        string[] strArray1 = textReader.ReadLine()?.Split(';');
        if (strArray1 == null)
            return;
        string[] strArray2 = strArray1[0].Split(':');
        string[] strArray3 = strArray1[1].Split(':');
        this.BackgroundColor = Color.FromArgb(int.Parse(strArray2[0]), int.Parse(strArray2[1]), int.Parse(strArray2[2]));
        this.ForegroundColor = Color.FromArgb(int.Parse(strArray3[0]), int.Parse(strArray3[1]), int.Parse(strArray3[2]));
        textReader.Close();
        textReader.Dispose();
    }

    public void SaveScheme(string masRoot)
    {
        TextWriter text = (TextWriter) File.CreateText(Path.Join(masRoot, "scheme.cfg"));
        text.Write($"{this.BackgroundColor.R}:{this.BackgroundColor.G}:{this.BackgroundColor.B}:;{this.ForegroundColor.R}:{this.ForegroundColor.G}:{this.ForegroundColor.B}:;");
        text.Close();
        text.Dispose();
    }

    public string BackgroundToHexString() => Scheme.ColorToHexString(this.BackgroundColor);

    public string ForegroundToHexString() => Scheme.ColorToHexString(this.ForegroundColor);

    private static string ColorToHexString(Color color)
    {
        byte num = color.R;
        string str1 = num.ToString("X").PadLeft(2, '0');
        num = color.G;
        string str2 = num.ToString("X").PadLeft(2, '0');
        num = color.B;
        string str3 = num.ToString("X").PadLeft(2, '0');
        return $"#{str1}{str2}{str3}";
    }
}