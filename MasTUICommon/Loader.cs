namespace MasTUICommon;

public class Loader
{
    public string StatusText
    {
        get;
        set
        {
            field = value;
            StatusTextChanged?.Invoke(value);
        }
    } = "";

    public delegate void StatusTextChangedHandler(string text);

    public event StatusTextChangedHandler? StatusTextChanged;

    private readonly char[] _loaders = ['/', '-', '\\', '|'];

    private int _loadIdx = 0;

    private DateTime _lastUpdate = DateTime.Now;

    public void PrintLoader()
    {
        if (DateTime.Now - _lastUpdate > TimeSpan.FromMilliseconds(100))
        {
            _loadIdx++;
            _lastUpdate = DateTime.Now;
            if (_loadIdx >= _loaders.Length) _loadIdx = 0;
        }

        var backup = Console.GetCursorPosition();
        var fullMsg = $" {_loaders[_loadIdx]}  {StatusText}";
        Console.SetCursorPosition(0, Console.WindowHeight / 2);
        Console.Write(fullMsg.PadBoth(Console.WindowWidth) + "\r");
    }
}