namespace MasTUICommon.Components;

public class Menu
{
    private Dictionary<int, MenuItem> Items { get; set; } = new();
    
    public Color DefaultColor { get; set; } = new() { BackgroundColor = 0, ForegroundColor = 15 };

    public Color ActiveColor { get; set; } = new() { BackgroundColor = 15, ForegroundColor = 0 };

    public int MarginLeft { get; set; } = 0;
    public int MarginTop { get; set; } = 0;
    
    public int TextPadding { get; set; } = 0;

    public int SelectedIndex
    {
        get;
        set
        {
            if ((value > Items.Count - 1 || value < 0) && Items.Count > 0) throw new IndexOutOfRangeException();
            field = value;
        }
    }

    public void AddItem(string title, MenuItem.ActionHandler handler, string? tooltip)
    {
        var mi = new MenuItem()
        {
            Title = title,
            Tooltip = tooltip,
        };
        mi.Action += handler;
        Items.Add(Items.Count, mi);
    }

    public void MoveUp()
    {
        switch (SelectedIndex)
        {
            case 0:
                SelectedIndex = Items.Count - 1;
                break;
            case > 0:
                SelectedIndex--;
                break;
        }
    }

    public void MoveDown()
    {
        if (SelectedIndex < Items.Count - 1)
        {
            SelectedIndex++;
        }
        else
        {
            SelectedIndex = 0;
        }
    }

    public void Draw()
    {
        foreach (var (actualIndex, item) in Items.Index())
        {
            var pCol = item.Key == SelectedIndex ? ActiveColor : DefaultColor;
            Console.SetCursorPosition(MarginLeft, MarginTop + actualIndex);
            var mChar = (item.Key == SelectedIndex) ? "<>" : "  ";
            ColorConsole.WriteLine($"~{pCol} {mChar[0]} {item.Value.Title.PadRight(TextPadding - 5)}{mChar[1]} ~--");
        }
        if (Items.Count == 0) return;
        var tip = Items[SelectedIndex].Tooltip;
        if (tip == null) return;
        Console.WriteLine();
        foreach (var sTip in Utils.WrapLines(tip, Console.WindowWidth - 1).Replace("\r\n", "\n").Split('\n'))
        {
            Console.WriteLine((" " + sTip).PadRight(Console.WindowWidth - 1));
        }
    }

    public void Execute()
    {
        Items[SelectedIndex].InvokeAction(this);
    }
}