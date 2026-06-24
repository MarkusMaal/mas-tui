namespace MasFlashDrv;

public abstract class ShowMsg
{
    public static void ShowDialog(string message)
    {
        const int minWidth = 50;
        var totalWidth = Math.Max(minWidth, message.Length + 4);
        var x = Console.WindowWidth / 2 - totalWidth / 2;
        var y = Console.WindowHeight / 2 - 2;
        Console.SetCursorPosition(x, y++);
        Console.Write("\u2554" + "".PadRight(totalWidth - 2, '\u2550') + "\u2557");
        Console.SetCursorPosition(x, y++);
        Console.Write("\u2551 " + message.PadRight(totalWidth - 3, ' ') + "\u2551");
        Console.SetCursorPosition(x, y++);
        Console.Write("\u255A" + "".PadRight(totalWidth - 2, '\u2550') + "\u255D");
        Console.ReadKey(true);
        x = Console.WindowWidth / 2 - totalWidth / 2;
        y = Console.WindowHeight / 2 - 2;
        for (var i = 0; i < 3; i++)
        {
            Console.SetCursorPosition(x, y++);
            Console.WriteLine("".PadRight(totalWidth));
        }
    }
}