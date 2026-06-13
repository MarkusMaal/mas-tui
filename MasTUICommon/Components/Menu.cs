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

    public void AddItem(string title, MenuItem.ActionHandler handler)
    {
        var mi = new MenuItem()
        {
            Title = title,
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
            ColorConsole.WriteLine($"~{pCol}{item.Value.Title.PadRight(TextPadding)}~--");
        }
    }

    public void Execute()
    {
        Items[SelectedIndex].InvokeAction(this);
    }
}