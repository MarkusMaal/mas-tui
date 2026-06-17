namespace MasTUICommon.Components;


public class Checkbox
{
    public required string Label { get; set; }

    public bool Value { get; set; }

    public char Key { get; set; }

    public required Color KeyColor { get; set; }

    public void Draw()
    {
        ColorConsole.Write($"~--[{(Value ? "~-A✓~--" : " ")}~--] ");
        var flag = false;
        foreach (var ch in Label)
        {
            if (ch == Key && !flag)
            {
                ColorConsole.Write($"~--(~{KeyColor}{Key}~--)");
                flag = true;
            }
            else
                Console.Write(ch);
        }
    }
}