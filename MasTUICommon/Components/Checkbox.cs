namespace MasTUICommon.Components;


public class Checkbox
{
    public required string Label { get; set; }

    public bool Value { get; set; }

    public char Key { get; set; }

    public required Color KeyColor { get; set; }

    public void Draw()
    {
        ColorConsole.Write($"~--[{(this.Value ? "~-A✓~--" : " ")}~--] ");
        bool flag = false;
        foreach (char ch in this.Label)
        {
            if ((int) ch == (int) this.Key && !flag)
            {
                ColorConsole.Write($"~--(~{this.KeyColor}{this.Key}~--)");
                flag = true;
            }
            else
                Console.Write(ch);
        }
    }
}