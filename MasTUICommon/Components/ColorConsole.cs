namespace MasTUICommon.Components;

public abstract class ColorConsole
{
    private static void HexStrToColor(string hex) // internal method, do not touch
    {
        var bg = hex[0];
        var fg = hex[1]; 
        if ((bg == '-') && (fg == '-'))
        {
            Console.ResetColor();
            return;
        }

        if (bg != '-')
        {
            var bgI = Convert.FromHexString("0" + bg)[0];
            Console.BackgroundColor = (ConsoleColor)bgI;
        }

        if (fg == '-') return;
        var fgI = Convert.FromHexString("0" + fg)[0];
        Console.ForegroundColor = (ConsoleColor)fgI;
    }

    /// <summary>
    /// Paints text characters inside the string to specific colors specified by my custom encoding and displays them.<br/><br/>
    ///
    /// Colors are encoded like so: ~[BG][FG], where BG represents a background color using a hex nibble (0-F)<br/>
    /// or dash character (-), FG is the same, but for foreground color. The first character of the encoded string MUST<br/>
    /// be a tilde (~).<br/><br/>
    ///
    /// Example: "~1FThis is white on blue.~-- This is default. ~-AThis is green on default.~4-This is default on red.~--"
    /// </summary>
    /// <param name="encoded">The encoded text</param>
    private static void DecodeColors(string encoded)
    {
        foreach (var _sect in encoded.Split('~'))
        {
            if (_sect.Length == 0) continue;
            var sect = _sect.Replace("::::", "~")[2..];
            var colorCode = _sect[..2].ToUpper(); 
            HexStrToColor(colorCode);
            Console.Write(sect);
        }
    }

    public static void WriteLine(string line)
    {
        DecodeColors(line);
        Console.WriteLine();
    }

    public static void Write(string line)
    {
        DecodeColors(line);
    }
}