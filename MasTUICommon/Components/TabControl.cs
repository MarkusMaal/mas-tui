namespace MasTUICommon.Components
{

    public class TabControl
    {
        public Dictionary<int, TabItem> TabItems { get; set; } = new ();

        public Color DefaultColor { get; set; } = new() { BackgroundColor = 0, ForegroundColor = 15 };

        public Color ActiveColor { get; set; } = new() { BackgroundColor = 15, ForegroundColor = 0 };

        public void AddTab(TabItem tabItem)
        {
            TabItems.Add(TabItems.Count, tabItem);
        }

        public int SelectedIndex
        {
            get;
            set
            {
                if (value > TabItems.Count - 1 || value < 0) throw new IndexOutOfRangeException();
                TabItems[SelectedIndex].InvokeLeave(this);
                TabItems[value].InvokeEnter(this);
                field = value;
            }
        } = 0;

        public void Draw()
        {
            var itemWidth = TabItems.Values.Max(v => v.Title.Length) + 6;
            Console.SetCursorPosition(0, 1);
            foreach (var item in TabItems)
            {
                var isSelected = item.Key == SelectedIndex; 
                var itemColor = item.Key == SelectedIndex ? ActiveColor : DefaultColor;
                var pad = itemWidth - 6;
                if (pad % 2 != 0) pad++;
                var rawOutput = item.Value.Title.PadBoth(pad);
                rawOutput = isSelected ? " < " + rawOutput + " > " : "   " + rawOutput + "   ";
                if (Console.CursorLeft > Console.WindowWidth - pad) Console.WriteLine();
                ColorConsole.Write($"~{itemColor}{rawOutput}~--");
            }
            Console.WriteLine("\n");
            TabItems[SelectedIndex].InvokeDraw(this);
        }
    }
}

// Source - https://stackoverflow.com/a/17590723
// Posted by David Colwell, modified by community. See post 'Timeline' for change history
// Retrieved 2026-06-13, License - CC BY-SA 3.0

namespace System
{
    public static class StringExtensions
    {
        public static string PadBoth(this string str, int length)
        {
            var spaces = length - str.Length;
            var padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(length);
        }
    }
}
