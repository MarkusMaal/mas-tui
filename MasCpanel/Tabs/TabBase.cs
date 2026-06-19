namespace MasCpanel.Tabs;

public abstract class TabBase
{
    public abstract bool VerifileOk { get; init; }
    public abstract void ReceiveKey(object sender, ConsoleKey key);
    public abstract void Draw(object sender, EventArgs e);
}