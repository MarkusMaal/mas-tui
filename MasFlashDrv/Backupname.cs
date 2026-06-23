namespace MasFlashDrv
{
    internal class Backupname
    {
        public static string ShowDialog()
        {
            const string message = "Et varukoopia lihtsamini ära tunda, saate sisestada sildi.";
            const string message2 = "Silt peab vastama failinimede reeglitele.";
            var totalWidth = message.Length + 4;
            var x = Console.WindowWidth / 2 - totalWidth / 2;
            var y = Console.WindowHeight / 2 - 4;
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2554\u2550" + "Varukoopia silt".PadRight(totalWidth - 3, '\u2550') + "\u2557");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551 " + message.PadRight(totalWidth - 3, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551 " + message2.PadRight(totalWidth - 3, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551" + "".PadRight(totalWidth - 2, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u2551" + "".PadRight(totalWidth - 2, ' ') + "\u2551");
            Console.SetCursorPosition(x, y++);
            Console.Write("\u255A" + "".PadRight(totalWidth - 2, '\u2550') + "\u255D");
            Console.SetCursorPosition(x + 2, y - 2);
            var o = (Console.ReadLine() ?? "markuse mälupulk").Replace("\0", "_");
            y -= 6;
            for (var i = 0; i < 6; i++)
            {
                Console.SetCursorPosition(x, y++);
                Console.WriteLine("".PadRight(totalWidth));
            }
            return o;
        }
    }
}
