namespace MasFlashDrv.Tabs
{
    internal abstract class TabBase
    {
        public abstract void ReceiveKey(object sender, ConsoleKey key);
        public abstract void Draw(object sender, EventArgs e);
    }
}
