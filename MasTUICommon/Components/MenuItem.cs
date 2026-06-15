namespace MasTUICommon.Components;

public class MenuItem
{
    public string Title { get; init; } = "";

    public string? Tooltip { get; init; } = "";
    
    public delegate void ActionHandler(object sender, EventArgs e);
    public event ActionHandler? Action;

    internal void InvokeAction(object sender)
    {
        Action?.Invoke(sender, EventArgs.Empty);
    }
}