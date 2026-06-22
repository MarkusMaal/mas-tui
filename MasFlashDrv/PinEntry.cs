namespace MasFlashDrv
{
    internal class PinEntry
    {
        public static string ShowDialog(string message)
        {
            const int minWidth = 50;
            var totalWidth = Math.Min(minWidth, message.Length + 4);
            var x = Console.WindowWidth / 2 - totalWidth / 2;
            var y = Console.WindowHeight / 2 - 3;
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2554" + "".PadRight(totalWidth - 2, '\u2550') + "\u2557");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551 " + message.PadRight(totalWidth - 3, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551" + "".PadRight(totalWidth - 2, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u255A" + "".PadRight(totalWidth - 2, '\u2550') + "\u255D");
            Console.SetCursorPosition(x + 2, y - 2);
            var o = "";
            var interrupt = false;
            while (!interrupt)
            {
                var readKey = Console.ReadKey(true);
                switch (readKey.Key)
                {
                    case ConsoleKey.Enter:
                        interrupt = true;
                        break;
                    case ConsoleKey.Escape:
                        return "";
                    case ConsoleKey.Backspace:
                        if (o.Length == 0) break;
                        if (readKey.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            o = "";
                            Console.SetCursorPosition(x + 2, y - 2);
                            Console.Write("".PadRight(totalWidth - 4));
                            Console.SetCursorPosition(x + 2, y - 2);
                            break;
                        }
                        o = o[..^1];
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                        break;
                    default:
                        if (o.Length >= totalWidth - 4) break;
                        // filter out everything except number keys
                        if ((int)readKey.Key < 48) break;
                        if ((int)readKey.Key > 57 && (int)readKey.Key < 96) break;
                        if ((int)readKey.Key > 105) break;
                        o += readKey.KeyChar;
                        Console.Write("*");
                        break;
                }
            }
            x = Console.WindowWidth / 2 - totalWidth / 2;
            y = Console.WindowHeight / 2 - 3;
            for (var i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(x, y++);
                Console.WriteLine("".PadRight(totalWidth));
            }
            return o;
        }
    }
}
