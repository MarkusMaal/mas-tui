namespace MasTUICommon.Components;

public class TabItem
{
    public string Title { get; set; } = "";
    
    public delegate void TabEnterHandler(object sender, EventArgs e);
    public event TabEnterHandler? TabEnter;
    
    public delegate void TabLeaveHandler(object sender, EventArgs e);
    public event TabLeaveHandler? TabLeave;
    
    public delegate void DrawHandler(object sender, EventArgs e);
    public event DrawHandler? Draw;
    
    public delegate void KeyDownHandler(object sender, ConsoleKey e);
    public event KeyDownHandler? KeyDown;
    
    internal void InvokeEnter(object sender)
    {
        TabEnter?.Invoke(sender, EventArgs.Empty);
    }

    internal void InvokeLeave(object sender)
    {
        TabLeave?.Invoke(sender, EventArgs.Empty);
    }

    internal void InvokeDraw(object sender)
    {
        Draw?.Invoke(sender, EventArgs.Empty);
    }

    public void InvokeKeyDown(object sender, ConsoleKey key)
    {
        KeyDown?.Invoke(sender, key);
    }
}